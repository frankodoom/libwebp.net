using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Libwebp.Net.Middleware;

/// <summary>
/// ASP.NET Core middleware that intercepts <c>multipart/form-data</c> requests
/// and automatically converts uploaded image files to WebP format before the
/// request reaches controllers / Razor Pages.
/// </summary>
public sealed class WebpUploadMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<WebpUploadMiddleware> _logger;

    public WebpUploadMiddleware(
        RequestDelegate next,
        ILogger<WebpUploadMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InvokeAsync(
        HttpContext context,
        IWebpConversionService conversionService,
        IOptions<WebpConversionOptions> optionsAccessor)
    {
        var options = optionsAccessor.Value;

        // Only process multipart/form-data requests
        if (!context.Request.HasFormContentType ||
            context.Request.ContentType == null ||
            !context.Request.ContentType.StartsWith("multipart/form-data", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        var form = await context.Request.ReadFormAsync();
        var originalFiles = form.Files;

        if (originalFiles.Count == 0)
        {
            await _next(context);
            return;
        }

        var convertedFiles = new List<IFormFile>(originalFiles.Count);
        var disposables = new List<MemoryStream>();
        bool anyConverted = false;

        try
        {
            foreach (var file in originalFiles)
            {
                if (ShouldConvert(context, file, options))
                {
                    try
                    {
                        using var inputStream = new MemoryStream();
                        await file.CopyToAsync(inputStream);
                        inputStream.Position = 0;

                        var (webpData, webpFileName) = await conversionService.ConvertAsync(
                            inputStream, file.FileName, context.RequestAborted);

                        disposables.Add(webpData);
                        convertedFiles.Add(new WebpFormFile(webpData, file.Name, webpFileName));
                        anyConverted = true;

                        _logger.LogInformation(
                            "Middleware converted {Original} ({OriginalSize}) → {WebP} ({WebPSize})",
                            file.FileName, file.Length, webpFileName, webpData.Length);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex,
                            "Failed to convert {FileName}, passing original through", file.FileName);
                        convertedFiles.Add(file);
                    }
                }
                else
                {
                    convertedFiles.Add(file);
                }
            }

            if (anyConverted)
            {
                // Build a new FormCollection with the converted files
                var newFileCollection = new FormFileCollection();
                newFileCollection.AddRange(convertedFiles);

                context.Request.Form = new FormCollection(
                    form.ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
                    newFileCollection);
            }

            await _next(context);
        }
        finally
        {
            foreach (var ms in disposables)
            {
                await ms.DisposeAsync();
            }
        }
    }

    /// <summary>
    /// Decides whether a file should be converted based on the configured options.
    /// </summary>
    private static bool ShouldConvert(HttpContext context, IFormFile file, WebpConversionOptions options)
    {
        // Already a WebP image – skip
        if (file.ContentType.Equals("image/webp", StringComparison.OrdinalIgnoreCase))
            return false;

        // Check content type whitelist
        if (!options.InputContentTypes.Contains(file.ContentType))
            return false;

        // Check file extension whitelist
        var ext = Path.GetExtension(file.FileName);
        if (string.IsNullOrEmpty(ext) || !options.InputExtensions.Contains(ext))
            return false;

        // Check max file size
        if (options.MaxInputSizeBytes.HasValue && file.Length > options.MaxInputSizeBytes.Value)
            return false;

        // Custom skip predicate
        if (options.SkipConversion?.Invoke(context, file) == true)
            return false;

        return true;
    }
}
