using Libwebp.Net.errors;
using Libwebp.Net.utility;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Libwebp.Net
{
    /// <summary>
    /// Receives a MemoryStream, encodes it to .webp format, and
    /// returns a FileStream of the result which can be saved to disk
    /// or downloaded by any .NET client application.
    /// </summary>
    public class WebpEncoder
    {
        private readonly WebPConfiguration _configuration;

        /// <summary>
        /// Creates a new encoder with the specified configuration.
        /// </summary>
        /// <param name="configuration">The WebP encoding configuration</param>
        public WebpEncoder(WebPConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// Encodes the input image to WebP format asynchronously.
        /// </summary>
        /// <param name="memoryStream">The input image as a MemoryStream</param>
        /// <param name="fileName">The original input file name</param>
        /// <returns>A FileStream of the encoded WebP file</returns>
        public async Task<FileStream> EncodeAsync(MemoryStream memoryStream, string fileName)
        {
            if (memoryStream == null)
                throw new ArgumentNullException(nameof(memoryStream));
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException(nameof(fileName));
            if (string.IsNullOrWhiteSpace(_configuration.OutputFileName))
                throw new OutputFileNameNotFoundException("Output file name not configured. Call Output() on WebpConfigurationBuilder.");

            // Sanitize file names to prevent command injection
            var sanitizedInputName = FileHelper.SanitizeFileName(fileName);
            var sanitizedOutputName = FileHelper.SanitizeFileName(_configuration.OutputFileName);

            var tempPath = Path.GetTempPath();
            var inputFilePath = Path.Combine(tempPath, sanitizedInputName);
            var outputFilePath = Path.Combine(tempPath, sanitizedOutputName);

            // Write input stream to temp location
            using (var fs = new FileStream(inputFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                memoryStream.Position = 0;
                await memoryStream.CopyToAsync(fs);
            }

            // Build the command from the configuration and file paths
            var commandBuilder = new CommandBuilder(_configuration, inputFilePath, outputFilePath);
            string command = commandBuilder.GetCommand();

            // Execute the encoding command and return the output file
            return await CommandExecutor.Execute(command, outputFilePath);
        }
    }
}
