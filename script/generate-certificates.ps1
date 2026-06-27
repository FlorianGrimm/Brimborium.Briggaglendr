$targetFolder=[System.IO.Path]::GetFullPath(
    [System.IO.Path]::Combine(
        $PSScriptRoot,
        "..",
        "certificate"
    )
)
[System.IO.Directory]::CreateDirectory($targetFolder)|Out-Null
[string]$rootCertFile = [System.IO.Path]::Combine($targetFolder, "root.cert")
[string]$rootPfxFile = [System.IO.Path]::Combine($targetFolder, "root.pfx")
[string]$certificatesJsonFile = [System.IO.Path]::Combine($targetFolder, "certificates.json")

$rootCertifcateSubject ="Brimborium.Briggaglendr"
$passwordForCertifcate = "Password@1"
$securePassword = ConvertTo-SecureString -String $passwordForCertifcate -Force -AsPlainText

# the hostnames the leaf certificates are issued for (based on the computer name,
# but distinct so the certificates can be told apart by subject name)
$httpsHostname  = $env:COMPUTERNAME
$clientHostname = "$($env:COMPUTERNAME)-client"

[string]$httpsPfxFile  = [System.IO.Path]::Combine($targetFolder, "https.pfx")
[string]$clientPfxFile = [System.IO.Path]::Combine($targetFolder, "client.pfx")

$certStoreLocation = "Cert:\CurrentUser\My"

# look up a certificate in the store by its subject common name; returns the
# newest still-valid match or $null if none exists
function Get-CertificateFromStore {
    param(
        [Parameter(Mandatory = $true)][string]$CommonName
    )
    Get-ChildItem -Path $certStoreLocation |
        Where-Object { $_.Subject -eq "CN=$CommonName" -and $_.NotAfter -gt (Get-Date) } |
        Sort-Object -Property NotAfter -Descending |
        Select-Object -First 1
}

# test if root certificate
$rootCertificate = Get-CertificateFromStore -CommonName $rootCertifcateSubject
if ($null -ne $rootCertificate) {
    Write-Host "Root certificate '$rootCertifcateSubject' already exists in '$certStoreLocation' - using thumbprint $($rootCertificate.Thumbprint)."
} else {
    # generate a self-signed certificate
    # make it a root certificate (CA: basic constraints + key usage CertSign)
    $rootCertificate = New-SelfSignedCertificate `
        -Subject "CN=$rootCertifcateSubject" `
        -KeyUsage CertSign, CRLSign, DigitalSignature `
        -KeyUsageProperty All `
        -KeyExportPolicy Exportable `
        -KeyLength 4096 `
        -HashAlgorithm SHA256 `
        -NotAfter (Get-Date).AddYears(10) `
        -CertStoreLocation $certStoreLocation `
        -TextExtension @("2.5.29.19={text}CA=true&pathlength=1")

    # export to $rootCertFile (public certificate only, DER/CER)
    Export-Certificate -Cert $rootCertificate -FilePath $rootCertFile -Type CERT | Out-Null

    # export to $rootPfxFile (certificate + private key, protected by password)
    Export-PfxCertificate -Cert $rootCertificate -FilePath $rootPfxFile -Password $securePassword | Out-Null

    Write-Host "Root certificate created: '$rootCertFile' and '$rootPfxFile'."
}

# generate a https certificate for hostname (server authentication, signed by root)
$httpsCertificate = Get-CertificateFromStore -CommonName $httpsHostname
if ($null -ne $httpsCertificate) {
    Write-Host "HTTPS certificate '$httpsHostname' already exists in '$certStoreLocation' - using thumbprint $($httpsCertificate.Thumbprint)."
} else {
    $httpsCertificate = New-SelfSignedCertificate `
        -Subject "CN=$httpsHostname" `
        -DnsName $httpsHostname `
        -Signer $rootCertificate `
        -KeyExportPolicy Exportable `
        -KeyLength 2048 `
        -HashAlgorithm SHA256 `
        -NotAfter (Get-Date).AddYears(2) `
        -CertStoreLocation $certStoreLocation `
        -TextExtension @("2.5.29.37={text}1.3.6.1.5.5.7.3.1")  # serverAuth
    Export-PfxCertificate -Cert $httpsCertificate -FilePath $httpsPfxFile -Password $securePassword | Out-Null
    Write-Host "HTTPS certificate created: '$httpsPfxFile'."
}

# generate a client certificate for hostname (client authentication, signed by root)
$clientCertificate = Get-CertificateFromStore -CommonName $clientHostname
if ($null -ne $clientCertificate) {
    Write-Host "Client certificate '$clientHostname' already exists in '$certStoreLocation' - using thumbprint $($clientCertificate.Thumbprint)."
} else {
    $clientCertificate = New-SelfSignedCertificate `
        -Subject "CN=$clientHostname" `
        -DnsName $clientHostname `
        -Signer $rootCertificate `
        -KeyExportPolicy Exportable `
        -KeyLength 2048 `
        -HashAlgorithm SHA256 `
        -NotAfter (Get-Date).AddYears(2) `
        -CertStoreLocation $certStoreLocation `
        -TextExtension @("2.5.29.37={text}1.3.6.1.5.5.7.3.2")  # clientAuth
    Export-PfxCertificate -Cert $clientCertificate -FilePath $clientPfxFile -Password $securePassword | Out-Null
    Write-Host "Client certificate created: '$clientPfxFile'."
}

# write the fingerprints (thumbprints) of the certificates to $certificatesJsonFile
$certificateFingerprints = [ordered]@{
    root   = $rootCertificate.Thumbprint
    https  = $httpsCertificate.Thumbprint
    client = $clientCertificate.Thumbprint
}
$certificateFingerprints | ConvertTo-Json | Set-Content -Path $certificatesJsonFile -Encoding UTF8
Write-Host "Certificate fingerprints written to '$certificatesJsonFile'."
