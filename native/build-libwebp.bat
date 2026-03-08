@echo off
REM Build libwebp from source as a shared library (DLL)
REM Requires: Visual Studio 2022 with C++ tools, CMake

set "MSVC_BIN=C:\Program Files\Microsoft Visual Studio\2022\Enterprise\VC\Tools\MSVC\14.41.34120\bin\Hostx64\x64"
set "MSVC_INC=C:\Program Files\Microsoft Visual Studio\2022\Enterprise\SDK\ScopeCppSDK\vc15\VC\include"
set "MSVC_LIB=C:\Program Files\Microsoft Visual Studio\2022\Enterprise\VC\Tools\MSVC\14.41.34120\lib\onecore\x64"

REM Windows SDK paths
set "WINSDK_INC=C:\Program Files (x86)\Windows Kits\10\Include\10.0.26100.0"
set "WINSDK_LIB=C:\Program Files (x86)\Windows Kits\10\Lib\10.0.26100.0"
set "WINSDK_BIN=C:\Program Files (x86)\Windows Kits\10\bin\10.0.26100.0\x64"

set "SRC=c:\Users\FrankOdoom\OneDrive - Accede\Desktop\Projects\Personal\libwebp.net\native\libwebp-src"
set "BUILD=c:\Users\FrankOdoom\OneDrive - Accede\Desktop\Projects\Personal\libwebp.net\native\build-win-x64"
set "CODECS=c:\Users\FrankOdoom\OneDrive - Accede\Desktop\Projects\Personal\libwebp.net\src\codecs\win"

echo [1/4] Setting up compiler environment...
set "PATH=%MSVC_BIN%;%WINSDK_BIN%;%PATH%"
set "INCLUDE=%MSVC_INC%;%WINSDK_INC%\ucrt;%WINSDK_INC%\um;%WINSDK_INC%\shared"
set "LIB=%MSVC_LIB%;%WINSDK_LIB%\ucrt\x64;%WINSDK_LIB%\um\x64"
where cl
where nmake

echo [2/4] Configuring CMake (NMake, Release, shared lib)...
if exist "%BUILD%" rmdir /s /q "%BUILD%"
mkdir "%BUILD%"
cmake -S "%SRC%" -B "%BUILD%" -G "NMake Makefiles" -DCMAKE_BUILD_TYPE=Release -DBUILD_SHARED_LIBS=ON -DCMAKE_C_COMPILER=cl -DCMAKE_RC_COMPILER=rc -DWEBP_BUILD_CWEBP=OFF -DWEBP_BUILD_DWEBP=OFF -DWEBP_BUILD_GIF2WEBP=OFF -DWEBP_BUILD_IMG2WEBP=OFF -DWEBP_BUILD_VWEBP=OFF -DWEBP_BUILD_WEBPINFO=OFF -DWEBP_BUILD_WEBPMUX=OFF -DWEBP_BUILD_EXTRAS=OFF -DWEBP_BUILD_ANIM_UTILS=OFF
if errorlevel 1 (
    echo ERROR: CMake configure failed
    exit /b 1
)

echo [3/4] Building...
cmake --build "%BUILD%" --config Release
if errorlevel 1 (
    echo ERROR: Build failed
    exit /b 1
)

echo [4/4] Copying DLLs to codecs\win\...
if not exist "%CODECS%" mkdir "%CODECS%"
for /R "%BUILD%" %%f in (webp.dll libwebp.dll) do (
    if exist "%%f" (
        copy /Y "%%f" "%CODECS%\libwebp.dll"
        echo Copied %%f to %CODECS%\libwebp.dll
    )
)
for /R "%BUILD%" %%f in (sharpyuv.dll libsharpyuv.dll) do (
    if exist "%%f" (
        copy /Y "%%f" "%CODECS%\libsharpyuv.dll"
        echo Copied %%f to %CODECS%\libsharpyuv.dll
    )
)

echo.
echo === Build complete ===
if exist "%CODECS%\libwebp.dll" (
    echo SUCCESS: %CODECS%\libwebp.dll
) else (
    echo WARNING: libwebp.dll not found in expected location
    echo Listing all DLLs in build dir:
    dir /s /b "%BUILD%\*.dll"
)
if exist "%CODECS%\libsharpyuv.dll" (
    echo SUCCESS: %CODECS%\libsharpyuv.dll
) else (
    echo WARNING: libsharpyuv.dll not found — libwebp.dll may fail to load at runtime
)
