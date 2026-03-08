using Microsoft.Extensions.DependencyInjection;

namespace Libwebp.Net.Middleware;

/// <summary>
/// Extension methods for registering WebP conversion services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the WebP conversion service and configures
    /// <see cref="WebpConversionOptions"/> using the supplied delegate.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Optional delegate to configure conversion options.</param>
    /// <returns>The same <see cref="IServiceCollection"/> for chaining.</returns>
    public static IServiceCollection AddWebpConversion(
        this IServiceCollection services,
        Action<WebpConversionOptions>? configure = null)
    {
        var optionsBuilder = services.AddOptions<WebpConversionOptions>();

        if (configure != null)
            optionsBuilder.Configure(configure);

        services.AddScoped<IWebpConversionService, WebpConversionService>();

        return services;
    }
}
