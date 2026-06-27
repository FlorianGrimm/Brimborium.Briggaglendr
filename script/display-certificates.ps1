
# display the certificates generated with generate-certificates.ps1

$rootCertifcateSubject = "Brimborium.Briggaglendr"

# the same hostnames/subjects generate-certificates.ps1 uses
$httpsHostname  = $env:COMPUTERNAME
$clientHostname = "$($env:COMPUTERNAME)-client"

$certStoreLocation = "Cert:\CurrentUser\My"

# the subject common names of the certificates that were generated
$commonNames = @($rootCertifcateSubject, $httpsHostname, $clientHostname)

# display the relevant certificates
$certificates = Get-ChildItem -Path $certStoreLocation |
    Where-Object { $commonNames -contains ($_.Subject -replace '^CN=', '') } |
    Sort-Object -Property Subject

if ($null -eq $certificates -or $certificates.Count -eq 0) {
    Write-Host "No matching certificates found in '$certStoreLocation'."
} else {
    Write-Host "Found the following certificates in '$certStoreLocation':"
    $certificates |
        Format-Table -AutoSize -Property `
            Subject, Thumbprint, NotBefore, NotAfter, `
            @{ Name = "HasPrivateKey"; Expression = { $_.HasPrivateKey } }, `
            @{ Name = "EnhancedKeyUsage"; Expression = { ($_.EnhancedKeyUsages | ForEach-Object { $_.FriendlyName }) -join ", " } }
}
