using Libwebp.Net.errors;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Libwebp.Net.utility
{
    /// <summary>
    /// Executes the libwebp codec to encode a file and returns
    /// the encoded file as a FileStream.
    /// </summary>
    public static class CommandExecutor
    {
        private static readonly object _codecLock = new object();

        /// <summary>
        /// Executes the cwebp command and returns the output file as a FileStream.
        /// </summary>
        /// <param name="command">The cwebp command arguments</param>
        /// <param name="outputFilePath">The expected output file path</param>
        /// <returns>FileStream of the encoded WebP file</returns>
        public static async Task<FileStream> Execute(string command, string outputFilePath)
        {
            string codecPath = GetCodecPath();

            var startInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                FileName = codecPath,
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = command,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };

            try
            {
                using var process = Process.Start(startInfo);
                if (process == null)
                    throw new CommandExecutionException("Failed to start the cwebp process.");

                string stderr = await process.StandardError.ReadToEndAsync();
                await process.WaitForExitAsync();

                if (process.ExitCode != 0)
                {
                    throw new CommandExecutionException(
                        $"cwebp exited with code {process.ExitCode}. Error: {stderr}");
                }
            }
            catch (CommandExecutionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CommandExecutionException(
                    "LibWebP.Net CommandExecutor Failed: " + ex.Message, ex);
            }

            if (!File.Exists(outputFilePath))
            {
                throw new CommandExecutionException(
                    $"Encoding completed but output file was not found at: {outputFilePath}");
            }

            return new FileStream(outputFilePath, FileMode.Open, FileAccess.Read);
        }

        /// <summary>
        /// Determines the correct codec path for the current platform,
        /// extracts the embedded binary if needed, and returns the path.
        /// </summary>
        private static string GetCodecPath()
        {
            string codecFileName;
            string resourceName;

            if (OperatingSystem.IsWindows())
            {
                codecFileName = "cwebp.exe";
                resourceName = "Libwebp.Net.codecs.win.cwebp.exe";
            }
            else if (OperatingSystem.IsLinux())
            {
                codecFileName = "cwebp";
                resourceName = "Libwebp.Net.codecs.linux.cwebp";
            }
            else if (OperatingSystem.IsMacOS())
            {
                codecFileName = "cwebp";
                resourceName = "Libwebp.Net.codecs.osx.cwebp";
            }
            else
            {
                throw new PlatformNotSupportedException(
                    "LibWebP.Net currently supports Windows, Linux, and macOS only.");
            }

            var codecPath = Path.Combine(Path.GetTempPath(), "libwebpnet", codecFileName);
            ExtractCodecIfNeeded(codecPath, resourceName);
            return codecPath;
        }

        /// <summary>
        /// Extracts the embedded codec binary to disk only if it doesn't already exist.
        /// On Unix platforms, sets the execute permission after extraction.
        /// </summary>
        private static void ExtractCodecIfNeeded(string targetPath, string resourceName)
        {
            if (File.Exists(targetPath))
                return;

            lock (_codecLock)
            {
                // Double-check after acquiring lock
                if (File.Exists(targetPath))
                    return;

                // Ensure the directory exists
                var dir = Path.GetDirectoryName(targetPath);
                if (!string.IsNullOrEmpty(dir))
                    Directory.CreateDirectory(dir);

                using var stream = Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream(resourceName);

                if (stream == null)
                    throw new CommandExecutionException(
                        $"Embedded codec resource '{resourceName}' not found. " +
                        $"Ensure the cwebp binary for your platform is included in the package.");

                using (var fs = new FileStream(targetPath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    stream.CopyTo(fs);
                }

                // Set execute permission on Unix platforms
                if (!OperatingSystem.IsWindows())
                {
                    SetExecutePermission(targetPath);
                }
            }
        }

        /// <summary>
        /// Sets the execute permission on a file (Unix/macOS only).
        /// Uses chmod +x via a short process invocation.
        /// </summary>
        private static void SetExecutePermission(string filePath)
        {
            try
            {
                using var chmod = Process.Start(new ProcessStartInfo
                {
                    FileName = "chmod",
                    Arguments = $"+x \"{filePath}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardError = true
                });
                chmod?.WaitForExit(5000);
            }
            catch
            {
                // chmod may not be available in some containers; 
                // the binary may still be executable if extracted correctly
            }
        }
    }
}
