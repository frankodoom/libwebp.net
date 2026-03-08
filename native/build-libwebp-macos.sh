#!/usr/bin/env bash
# Build libwebp from source as a shared library (.dylib) for macOS
# Requires: Xcode CLI tools (clang), cmake
# Usage: ./build-libwebp-macos.sh

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
SRC="$SCRIPT_DIR/libwebp-src"
BUILD="$SCRIPT_DIR/build-osx-arm64"
CODECS="$SCRIPT_DIR/../src/codecs/osx"

# Detect architecture
ARCH="$(uname -m)"
if [ "$ARCH" = "x86_64" ]; then
    CMAKE_ARCH_FLAGS="-DCMAKE_OSX_ARCHITECTURES=x86_64"
    echo "Building for macOS x86_64 (Intel)"
else
    CMAKE_ARCH_FLAGS="-DCMAKE_OSX_ARCHITECTURES=arm64"
    echo "Building for macOS arm64 (Apple Silicon)"
fi

# Clone source if not present
if [ ! -d "$SRC" ]; then
    echo "[0/4] Cloning libwebp source..."
    git clone --depth 1 --branch v1.5.0 https://github.com/webmproject/libwebp.git "$SRC"
fi

echo "[1/4] Setting up build directory..."
rm -rf "$BUILD"
mkdir -p "$BUILD"

echo "[2/4] Configuring CMake (Unix Makefiles, Release, shared lib)..."
cmake -S "$SRC" -B "$BUILD" \
    -G "Unix Makefiles" \
    -DCMAKE_BUILD_TYPE=Release \
    -DBUILD_SHARED_LIBS=ON \
    $CMAKE_ARCH_FLAGS \
    -DWEBP_BUILD_CWEBP=OFF \
    -DWEBP_BUILD_DWEBP=OFF \
    -DWEBP_BUILD_GIF2WEBP=OFF \
    -DWEBP_BUILD_IMG2WEBP=OFF \
    -DWEBP_BUILD_VWEBP=OFF \
    -DWEBP_BUILD_WEBPINFO=OFF \
    -DWEBP_BUILD_WEBPMUX=OFF \
    -DWEBP_BUILD_EXTRAS=OFF \
    -DWEBP_BUILD_ANIM_UTILS=OFF

echo "[3/4] Building..."
cmake --build "$BUILD" --config Release -- -j"$(sysctl -n hw.ncpu)"

echo "[4/4] Copying shared libraries to codecs/osx/..."
mkdir -p "$CODECS"

# Copy libwebp.dylib
LIBWEBP=$(find "$BUILD" -name "libwebp*.dylib" -not -type l | head -1)
if [ -n "$LIBWEBP" ]; then
    cp "$LIBWEBP" "$CODECS/libwebp.dylib"
    echo "  -> codecs/osx/libwebp.dylib ($(du -h "$CODECS/libwebp.dylib" | cut -f1))"
else
    echo "ERROR: libwebp.dylib not found in build output" >&2
    exit 1
fi

# Copy libsharpyuv.dylib (dependency)
LIBSHARPYUV=$(find "$BUILD" -name "libsharpyuv*.dylib" -not -type l | head -1)
if [ -n "$LIBSHARPYUV" ]; then
    cp "$LIBSHARPYUV" "$CODECS/libsharpyuv.dylib"
    echo "  -> codecs/osx/libsharpyuv.dylib ($(du -h "$CODECS/libsharpyuv.dylib" | cut -f1))"
else
    echo "WARNING: libsharpyuv.dylib not found — libwebp may fail to load at runtime"
fi

echo ""
echo "=== Build complete ==="
ls -lh "$CODECS/"
