<#
.SYNOPSIS
    Downloads the latest libwebp cwebp binaries from Google for all platforms.
.DESCRIPTION
    Downloads and extracts the cwebp encoder binaries for Windows (x64),
    Linux (x86-64), and macOS (arm64 + x86-64) from Google's official
    libwebp releases.
    
    Run this script from the src/codecs directory.
.NOTES
    Latest version: 1.5.0 (stable) as of 2025
    Update $version variable when newer releases are available.
    Check https://developers.google.com/speed/webp/download for the latest.
#>

$version = "1.5.0"
$baseUrl = "https://storage.googleapis.com/downloads.webmproject.org/releases/webp"
$scriptDir = $PSScriptRoot

Write-Host "Downloading libwebp $version cwebp binaries..." -ForegroundColor Cyan

# Windows x64
Write-Host "`n[1/3] Downloading Windows x64..." -ForegroundColor Yellow
$winZip = Join-Path $scriptDir "libwebp-win.zip"
$winUrl = "$baseUrl/libwebp-$version-windows-x64.zip"
Invoke-WebRequest -Uri $winUrl -OutFile $winZip
Expand-Archive -Path $winZip -DestinationPath $scriptDir -Force
$winCwebp = Join-Path $scriptDir "libwebp-$version-windows-x64\bin\cwebp.exe"
Copy-Item $winCwebp -Destination (Join-Path $scriptDir "win\cwebp.exe") -Force
Remove-Item $winZip -Force
Remove-Item (Join-Path $scriptDir "libwebp-$version-windows-x64") -Recurse -Force
Write-Host "  -> win\cwebp.exe" -ForegroundColor Green

# Linux x86-64
Write-Host "`n[2/3] Downloading Linux x86-64..." -ForegroundColor Yellow
$linuxTar = Join-Path $scriptDir "libwebp-linux.tar.gz"
$linuxUrl = "$baseUrl/libwebp-$version-linux-x86-64.tar.gz"
Invoke-WebRequest -Uri $linuxUrl -OutFile $linuxTar
tar -xzf $linuxTar -C $scriptDir
$linuxCwebp = Join-Path $scriptDir "libwebp-$version-linux-x86-64\bin\cwebp"
Copy-Item $linuxCwebp -Destination (Join-Path $scriptDir "linux\cwebp") -Force
Remove-Item $linuxTar -Force
Remove-Item (Join-Path $scriptDir "libwebp-$version-linux-x86-64") -Recurse -Force
Write-Host "  -> linux/cwebp" -ForegroundColor Green

# macOS arm64 (Apple Silicon)
Write-Host "`n[3/3] Downloading macOS arm64..." -ForegroundColor Yellow
$macTar = Join-Path $scriptDir "libwebp-mac.tar.gz"
$macUrl = "$baseUrl/libwebp-$version-mac-arm64.tar.gz"
Invoke-WebRequest -Uri $macUrl -OutFile $macTar
tar -xzf $macTar -C $scriptDir
$macCwebp = Join-Path $scriptDir "libwebp-$version-mac-arm64\bin\cwebp"
New-Item -ItemType Directory -Path (Join-Path $scriptDir "osx") -Force | Out-Null
Copy-Item $macCwebp -Destination (Join-Path $scriptDir "osx\cwebp") -Force
Remove-Item $macTar -Force
Remove-Item (Join-Path $scriptDir "libwebp-$version-mac-arm64") -Recurse -Force
Write-Host "  -> osx/cwebp" -ForegroundColor Green

Write-Host "`nDone! All cwebp $version binaries downloaded." -ForegroundColor Cyan
Write-Host "Files:"
Write-Host "  win/cwebp.exe   - Windows x64"
Write-Host "  linux/cwebp     - Linux x86-64"
Write-Host "  osx/cwebp       - macOS arm64"
