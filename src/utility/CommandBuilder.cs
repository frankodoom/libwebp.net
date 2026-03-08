using System;
using System.Collections.Generic;
using System.Text;

namespace Libwebp.Net.utility
{
    /// <summary>
    /// Constructs the cwebp command-line arguments from the WebP configuration
    /// and file paths.
    /// © 2021 Frank Arkhurst Odoom
    /// </summary>
    public class CommandBuilder
    {
        private readonly string _arguments;

        public CommandBuilder(WebPConfiguration configuration, string inputFilePath, string outputFilePath)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));
            if (string.IsNullOrWhiteSpace(inputFilePath))
                throw new ArgumentNullException(nameof(inputFilePath));
            if (string.IsNullOrWhiteSpace(outputFilePath))
                throw new ArgumentNullException(nameof(outputFilePath));

            string[] args = new string[]
            {
                $"\"{inputFilePath}\"",
                configuration.Preset,
                configuration.Lossless,
                configuration.LosslessPreset,
                configuration.QualityFactor,
                configuration.AlphaQ,
                configuration.CompressionMethod,
                configuration.NumberOfSegments,
                configuration.TargetSize,
                configuration.TargetPSNR,
                configuration.InputSize,
                configuration.SpatialNoiseShaping,
                configuration.Filter,
                configuration.Sharpness,
                configuration.Strong,
                configuration.SharpYuv,
                configuration.PartitionLimit,
                configuration.Pass,
                configuration.Crop,
                configuration.Resize,
                configuration.MultiThreading,
                configuration.LowMemory,
                configuration.AlphaMethod,
                configuration.AlphaFilter,
                configuration.Exact,
                configuration.NoAlpha,
                configuration.NearLossless,
                configuration.Hint,
                configuration.Metadata,
                $"{CommandPrefix.Output}\"{outputFilePath}\""
            };

            _arguments = BuildArguments(args);
        }

        private static string BuildArguments(string[] args)
        {
            var userArgs = new List<string>();
            foreach (var param in args)
            {
                if (!string.IsNullOrWhiteSpace(param))
                    userArgs.Add(param);
            }
            return string.Join(" ", userArgs);
        }

        /// <summary>
        /// Get the command arguments built by the Builder.
        /// These are passed as Arguments to ProcessStartInfo (FileName is the codec binary).
        /// </summary>
        /// <returns>The argument string for cwebp</returns>
        public string GetCommand() => _arguments;
    }
}
