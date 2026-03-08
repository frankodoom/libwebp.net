using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libwebp.Net.utility
{
    /// <summary>
    /// Utility class for file name sanitization and validation.
    /// </summary>
    public static class FileHelper
    {
        /// <summary>
        /// Sanitizes a file name by removing path traversal and invalid characters
        /// to prevent command injection attacks.
        /// </summary>
        /// <param name="fileName">The file name to sanitize</param>
        /// <returns>A sanitized file name safe for use in commands</returns>
        /// <exception cref="ArgumentException">If the file name is empty after sanitization</exception>
        public static string SanitizeFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("File name cannot be null or empty.", nameof(fileName));

            // Extract just the file name, removing any path components
            var name = Path.GetFileName(fileName);

            // Remove any invalid file name characters
            var invalidChars = Path.GetInvalidFileNameChars();
            var sanitized = new string(name.Where(c => !invalidChars.Contains(c)).ToArray());

            if (string.IsNullOrWhiteSpace(sanitized))
                throw new ArgumentException("File name contains only invalid characters.", nameof(fileName));

            return sanitized;
        }
    }
}
