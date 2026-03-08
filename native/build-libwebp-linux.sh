#!/usr/bin/env bash
# Build libwebp from source as a shared library (.so) for Linux x86-64
# Requires: gcc/g++, cmake, make
# Usage: ./build-libwebp-linux.sh

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
SRC="$SCRIPT_DIR/libwebp-src"
BUILD="$SCRIPT_DIR/build-linux-x64"
CODECS="$SCRIPT_DIR/../src/codecs/linux"

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
cmake --build "$BUILD" --config Release -- -j"$(nproc)"

echo "[4/4] Copying shared libraries to codecs/linux/..."
mkdir -p "$CODECS"

# Copy libwebp.so (use the symlink-resolved file)
LIBWEBP=$(find "$BUILD" -name "libwebp.so*" -not -type l | head -1)
if [ -n "$LIBWEBP" ]; then
    cp "$LIBWEBP" "$CODECS/libwebp.so"
    echo "  -> codecs/linux/libwebp.so ($(du -h "$CODECS/libwebp.so" | cut -f1))"
else
    echo "ERROR: libwebp.so not found in build output" >&2
    exit 1
fi

# Copy libsharpyuv.so (dependency)
LIBSHARPYUV=$(find "$BUILD" -name "libsharpyuv.so*" -not -type l | head -1)
if [ -n "$LIBSHARPYUV" ]; then
    cp "$LIBSHARPYUV" "$CODECS/libsharpyuv.so"
    echo "  -> codecs/linux/libsharpyuv.so ($(du -h "$CODECS/libsharpyuv.so" | cut -f1))"
else
    echo "WARNING: libsharpyuv.so not found — libwebp.dll may fail to load at runtime"
fi

echo ""
echo "=== Build complete ==="
ls -lh "$CODECS/"
