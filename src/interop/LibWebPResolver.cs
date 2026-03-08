#nullable enable
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Libwebp.Net.Interop
{
    /// <summary>
    /// Registers a custom <see cref="NativeLibrary"/> resolver so DllImport("libwebp")
    /// can locate the native shared library stored in the codecs/ folder.
    /// 
    /// Resolution order:
    /// 1. Assembly output directory (the lib is copied there by the build via CopyToOutputDirectory).
    /// 2. codecs/{platform}/ relative to the source tree (for development / test runs).
    /// 3. Fall through to the default .NET probing (runtimes/{rid}/native/, system paths, etc.).
    /// </summary>
    internal static class LibWebPResolver
    {
        private static bool _registered;
        private static readonly object _lock = new();

        /// <summary>
        /// Call once (idempotent) to register the resolver.
        /// Typically invoked from <c>NativeEncoder</c> or the first P/Invoke call site.
        /// </summary>
        public static void EnsureRegistered()
        {
            if (_registered) return;
            lock (_lock)
            {
                if (_registered) return;
                NativeLibrary.SetDllImportResolver(
                    typeof(LibWebPNative).Assembly,
                    ResolveLibWebP);
                _registered = true;
            }
        }

        private static nint ResolveLibWebP(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
        {
            // Only intercept our own library name.
            if (!libraryName.Equals("libwebp", StringComparison.OrdinalIgnoreCase))
                return nint.Zero; // let default resolver handle other libs

            // Determine the platform-specific filename and codecs subfolder.
            string fileName;
            string codecsSubfolder;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                fileName = "libwebp.dll";
                codecsSubfolder = "win";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                fileName = "libwebp.so";
                codecsSubfolder = "linux";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                fileName = "libwebp.dylib";
                codecsSubfolder = "osx";
            }
            else
            {
                return nint.Zero; // unsupported platform — let default probing try
            }

            // 1. Check next to the executing assembly (build output directory).
            string assemblyDir = Path.GetDirectoryName(assembly.Location) ?? ".";
            string candidate = Path.Combine(assemblyDir, fileName);
            if (NativeLibrary.TryLoad(candidate, out nint handle))
                return handle;

            // 2. Check codecs/{platform}/ relative to the source tree.
            //    Walk up from the assembly directory looking for the codecs folder.
            string? dir = assemblyDir;
            for (int i = 0; i < 8 && dir != null; i++)
            {
                string codecsPath = Path.Combine(dir, "codecs", codecsSubfolder, fileName);
                if (NativeLibrary.TryLoad(codecsPath, out handle))
                    return handle;
                dir = Path.GetDirectoryName(dir);
            }

            // 3. Fall through to default .NET resolution (system paths, LD_LIBRARY_PATH, etc.).
            return nint.Zero;
        }
    }
}
