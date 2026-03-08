using Microsoft.AspNetCore.Builder;

namespace Libwebp.Net.Middleware;

/// <summary>
/// Extension methods for adding WebP conversion middleware to the pipeline.
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Adds middleware that automatically converts uploaded images to WebP format.
    /// Place this <b>before</b> <c>MapControllers</c> / <c>MapRazorPages</c> so that
    /// controllers receive the converted files.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The same <see cref="IApplicationBuilder"/> for chaining.</returns>
    public static IApplicationBuilder UseWebpUploadConversion(this IApplicationBuilder app)
    {
        return app.UseMiddleware<WebpUploadMiddleware>();
    }
}
