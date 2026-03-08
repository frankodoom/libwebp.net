<#
.SYNOPSIS
    Builds the native libwebp shared libraries from Google's open-source C code.
.DESCRIPTION
    Clones (or reuses) the libwebp v1.5.0 source from GitHub, then compiles platform-
    specific shared libraries using CMake:

      codecs/win/libwebp.dll          + libsharpyuv.dll   (Windows x64)
      codecs/linux/libwebp.so         + libsharpyuv.so    (Linux x86-64)
      codecs/osx/libwebp.dylib        + libsharpyuv.dylib (macOS arm64)

    On Windows this script delegates to native/build-libwebp.bat.
    On Linux/macOS it delegates to the corresponding shell script.

    Google's pre-built release archives do NOT ship shared libraries (.dll/.so/.dylib),
    only CLI tools and static .lib/.a files. Building from source is required.

    Prerequisites:
      - Git, CMake (3.20+)
      - Windows: Visual Studio 2022 with C++ tools
      - Linux:   gcc/g++, make
      - macOS:   Xcode CLI tools (clang), make

.NOTES
    Run from: src/codecs/  (or anywhere — the script uses $PSScriptRoot)
    Source:   https://github.com/webmproject/libwebp (tag v1.5.0)
#>

param(
    [string]$Version = "1.5.0"
)

$ErrorActionPreference = "Stop"
$scriptDir = $PSScriptRoot
$nativeDir = Join-Path (Split-Path $scriptDir -Parent) "native"
$srcDir    = Join-Path $nativeDir "libwebp-src"

Write-Host "=== libwebp $Version native build ===" -ForegroundColor Cyan
Write-Host "Script dir : $scriptDir"              -ForegroundColor Gray
Write-Host "Native dir : $nativeDir"               -ForegroundColor Gray

# ── Ensure source is cloned ──
if (-not (Test-Path $srcDir)) {
    Write-Host "`nCloning libwebp v$Version from GitHub..." -ForegroundColor Yellow
    git clone --depth 1 --branch "v$Version" https://github.com/webmproject/libwebp.git "$srcDir"
    if ($LASTEXITCODE -ne 0) { throw "git clone failed" }
} else {
    Write-Host "`nSource already present at $srcDir" -ForegroundColor Green
}

# ── Detect platform and delegate ──
if ($IsWindows -or ($env:OS -eq "Windows_NT")) {
    Write-Host "`nBuilding for Windows x64..." -ForegroundColor Yellow
    $batFile = Join-Path $nativeDir "build-libwebp.bat"
    if (-not (Test-Path $batFile)) { throw "Build script not found: $batFile" }
    & cmd /c "`"$batFile`""
    if ($LASTEXITCODE -ne 0) { throw "Windows build failed (exit $LASTEXITCODE)" }
}
elseif ($IsLinux) {
    Write-Host "`nBuilding for Linux x86-64..." -ForegroundColor Yellow
    $shFile = Join-Path $nativeDir "build-libwebp-linux.sh"
    if (-not (Test-Path $shFile)) { throw "Build script not found: $shFile" }
    & bash "$shFile"
    if ($LASTEXITCODE -ne 0) { throw "Linux build failed (exit $LASTEXITCODE)" }
}
elseif ($IsMacOS) {
    Write-Host "`nBuilding for macOS..." -ForegroundColor Yellow
    $shFile = Join-Path $nativeDir "build-libwebp-macos.sh"
    if (-not (Test-Path $shFile)) { throw "Build script not found: $shFile" }
    & bash "$shFile"
    if ($LASTEXITCODE -ne 0) { throw "macOS build failed (exit $LASTEXITCODE)" }
}
else {
    throw "Unsupported platform. Build manually using CMake."
}

# ── Summary ──
Write-Host "`n── Summary ──" -ForegroundColor Cyan
$found = 0
$platforms = @(
    @{ Dir = "win";   Files = @("libwebp.dll", "libsharpyuv.dll");     Label = "Windows x64" },
    @{ Dir = "linux"; Files = @("libwebp.so", "libsharpyuv.so");       Label = "Linux x86-64" },
    @{ Dir = "osx";   Files = @("libwebp.dylib", "libsharpyuv.dylib"); Label = "macOS arm64" }
)
foreach ($p in $platforms) {
    $dir = Join-Path $scriptDir $p.Dir
    $hasAll = $true
    foreach ($f in $p.Files) {
        $path = Join-Path $dir $f
        if (Test-Path $path) {
            $size = [math]::Round((Get-Item $path).Length / 1KB, 1)
            Write-Host "  [$($p.Label)] $($p.Dir)/$f ($size KB)" -ForegroundColor Green
        } else {
            $hasAll = $false
        }
    }
    if ($hasAll) { $found++ }
    elseif (-not (Test-Path $dir)) {
        Write-Host "  [$($p.Label)] not built (run on that platform)" -ForegroundColor DarkGray
    }
}
Write-Host "`n$found/3 platforms ready." -ForegroundColor $(if ($found -eq 3) { "Green" } else { "Yellow" })
Write-Host "Run 'dotnet build' to verify the project compiles." -ForegroundColor Gray
