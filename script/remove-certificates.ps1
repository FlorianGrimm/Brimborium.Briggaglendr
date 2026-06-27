# removes the certificates from generated with generate-certificates.ps1

$targetFolder=[System.IO.Path]::GetFullPath(
    [System.IO.Path]::Combine(
        $PSScriptRoot,
        "..",
        "certificate"
    )
)

$rootCertifcateSubject = "Brimborium.Briggaglendr"

# the same hostnames/subjects generate-certificates.ps1 uses
$httpsHostname  = $env:COMPUTERNAME
$clientHostname = "$($env:COMPUTERNAME)-client"

$certStoreLocation = "Cert:\CurrentUser\My"

# the subject common names of the certificates that were generated
$commonNames = @($rootCertifcateSubject, $httpsHostname, $clientHostname)

# the files that were exported
$exportedFiles = @(
    [System.IO.Path]::Combine($targetFolder, "root.cert"),
    [System.IO.Path]::Combine($targetFolder, "root.pfx"),
    [System.IO.Path]::Combine($targetFolder, "https.pfx"),
    [System.IO.Path]::Combine($targetFolder, "client.pfx"),
    [System.IO.Path]::Combine($targetFolder, "certificates.json")
)

# display the relevant certificates
$certificates = Get-ChildItem -Path $certStoreLocation |
    Where-Object { $commonNames -contains ($_.Subject -replace '^CN=', '') }

if ($null -eq $certificates -or $certificates.Count -eq 0) {
    Write-Host "No matching certificates found in '$certStoreLocation'."
} else {
    Write-Host "Found the following certificates in '$certStoreLocation':"
    $certificates | Format-Table -Property Subject, Thumbprint, NotAfter | Out-String | Write-Host

    # remove it
    foreach ($certificate in $certificates) {
        Remove-Item -Path "$certStoreLocation\$($certificate.Thumbprint)" -Force
        Write-Host "Removed certificate '$($certificate.Subject)' (thumbprint $($certificate.Thumbprint))."
    }
}

# remove the exported files
foreach ($file in $exportedFiles) {
    if ([System.IO.File]::Exists($file)) {
        Remove-Item -Path $file -Force
        Write-Host "Removed file '$file'."
    }
}