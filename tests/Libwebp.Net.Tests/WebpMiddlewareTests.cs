using Libwebp.Net.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Libwebp.Net.Tests;

public class WebpConversionOptionsTests
{
    [Fact]
    public void Defaults_InputContentTypes_ContainsExpectedTypes()
    {
        var options = new WebpConversionOptions();

        Assert.Contains("image/jpeg", options.InputContentTypes);
        Assert.Contains("image/png", options.InputContentTypes);
        Assert.Contains("image/gif", options.InputContentTypes);
        Assert.Contains("image/tiff", options.InputContentTypes);
        Assert.Contains("image/bmp", options.InputContentTypes);
        Assert.Equal(5, options.InputContentTypes.Count);
    }

    [Fact]
    public void Defaults_InputExtensions_ContainsExpectedExtensions()
    {
        var options = new WebpConversionOptions();

        Assert.Contains(".jpg", options.InputExtensions);
        Assert.Contains(".jpeg", options.InputExtensions);
        Assert.Contains(".png", options.InputExtensions);
        Assert.Contains(".gif", options.InputExtensions);
        Assert.Contains(".tiff", options.InputExtensions);
        Assert.Contains(".tif", options.InputExtensions);
        Assert.Contains(".bmp", options.InputExtensions);
        Assert.Equal(7, options.InputExtensions.Count);
    }

    [Fact]
    public void Defaults_MaxInputSizeBytes_IsNull()
    {
        var options = new WebpConversionOptions();
        Assert.Null(options.MaxInputSizeBytes);
    }

    [Fact]
    public void Defaults_SkipConversion_IsNull()
    {
        var options = new WebpConversionOptions();
        Assert.Null(options.SkipConversion);
    }

    [Fact]
    public void Defaults_BoolOptions_AreFalse()
    {
        var options = new WebpConversionOptions();

        Assert.False(options.Lossless);
        Assert.False(options.SharpYuv);
        Assert.False(options.NoAlpha);
        Assert.False(options.Exact);
        Assert.False(options.MultiThreading);
        Assert.False(options.LowMemory);
    }

    [Fact]
    public void Defaults_NullableOptions_AreNull()
    {
        var options = new WebpConversionOptions();

        Assert.Null(options.QualityFactor);
        Assert.Null(options.Preset);
        Assert.Null(options.CompressionMethod);
        Assert.Null(options.LosslessPreset);
        Assert.Null(options.Pass);
        Assert.Null(options.TargetSize);
        Assert.Null(options.TargetPSNR);
        Assert.Null(options.NearLossless);
        Assert.Null(options.Hint);
        Assert.Null(options.Filter);
        Assert.Null(options.Sharpness);
        Assert.Null(options.UseStrongFilter);
        Assert.Null(options.SpatialNoiseShaping);
        Assert.Null(options.NumberOfSegments);
        Assert.Null(options.PartitionLimit);
        Assert.Null(options.AlphaQ);
        Assert.Null(options.AlphaMethod);
        Assert.Null(options.AlphaFilter);
        Assert.Null(options.Metadata);
    }

    [Fact]
    public void InputContentTypes_CaseInsensitive()
    {
        var options = new WebpConversionOptions();

        Assert.Contains("IMAGE/JPEG", options.InputContentTypes);
        Assert.Contains("Image/Png", options.InputContentTypes);
    }

    [Fact]
    public void InputExtensions_CaseInsensitive()
    {
        var options = new WebpConversionOptions();

        Assert.Contains(".JPG", options.InputExtensions);
        Assert.Contains(".Png", options.InputExtensions);
    }
}

public class WebpUploadMiddlewareTests
{
    private static WebpConversionOptions DefaultOptions() => new();

    private static IOptions<WebpConversionOptions> OptionsWrapper(WebpConversionOptions? opts = null) =>
        Options.Create(opts ?? DefaultOptions());

    [Fact]
    public async Task NonMultipart_PassesThrough()
    {
        bool nextCalled = false;
        var middleware = new WebpUploadMiddleware(
            _ => { nextCalled = true; return Task.CompletedTask; },
            NullLogger<WebpUploadMiddleware>.Instance);

        var context = new DefaultHttpContext();
        context.Request.ContentType = "application/json";

        await middleware.InvokeAsync(
            context,
            Mock.Of<IWebpConversionService>(),
            OptionsWrapper());

        Assert.True(nextCalled);
    }

    [Fact]
    public async Task MultipartWithNoFiles_PassesThrough()
    {
        bool nextCalled = false;
        var middleware = new WebpUploadMiddleware(
            _ => { nextCalled = true; return Task.CompletedTask; },
            NullLogger<WebpUploadMiddleware>.Instance);

        var context = new DefaultHttpContext();
        context.Request.ContentType = "multipart/form-data; boundary=----test";
        context.Request.Form = new FormCollection(
            new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>(),
            new FormFileCollection());

        await middleware.InvokeAsync(
            context,
            Mock.Of<IWebpConversionService>(),
            OptionsWrapper());

        Assert.True(nextCalled);
    }

    [Fact]
    public async Task AlreadyWebp_IsNotConverted()
    {
        var serviceMock = new Mock<IWebpConversionService>();
        bool nextCalled = false;

        var middleware = new WebpUploadMiddleware(
            _ => { nextCalled = true; return Task.CompletedTask; },
            NullLogger<WebpUploadMiddleware>.Instance);

        var webpFile = CreateFormFile("photo.webp", "image/webp", new byte[] { 0x01 });

        var context = new DefaultHttpContext();
        context.Request.ContentType = "multipart/form-data; boundary=----test";
        var files = new FormFileCollection { webpFile };
        context.Request.Form = new FormCollection(
            new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>(), files);

        await middleware.InvokeAsync(context, serviceMock.Object, OptionsWrapper());

        Assert.True(nextCalled);
        serviceMock.Verify(
            s => s.ConvertAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task UnsupportedContentType_IsNotConverted()
    {
        var serviceMock = new Mock<IWebpConversionService>();

        var middleware = new WebpUploadMiddleware(
            _ => Task.CompletedTask,
            NullLogger<WebpUploadMiddleware>.Instance);

        var svgFile = CreateFormFile("icon.svg", "image/svg+xml", new byte[] { 0x01 });

        var context = new DefaultHttpContext();
        context.Request.ContentType = "multipart/form-data; boundary=----test";
        var files = new FormFileCollection { svgFile };
        context.Request.Form = new FormCollection(
            new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>(), files);

        await middleware.InvokeAsync(context, serviceMock.Object, OptionsWrapper());

        serviceMock.Verify(
            s => s.ConvertAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ExceedMaxSize_IsNotConverted()
    {
        var serviceMock = new Mock<IWebpConversionService>();
        var options = new WebpConversionOptions { MaxInputSizeBytes = 5 };

        var middleware = new WebpUploadMiddleware(
            _ => Task.CompletedTask,
            NullLogger<WebpUploadMiddleware>.Instance);

        var bigFile = CreateFormFile("big.jpg", "image/jpeg", new byte[10]);

        var context = new DefaultHttpContext();
        context.Request.ContentType = "multipart/form-data; boundary=----test";
        var files = new FormFileCollection { bigFile };
        context.Request.Form = new FormCollection(
            new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>(), files);

        await middleware.InvokeAsync(context, serviceMock.Object, OptionsWrapper(options));

        serviceMock.Verify(
            s => s.ConvertAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task SkipPredicate_PreventsConversion()
    {
        var serviceMock = new Mock<IWebpConversionService>();
        var options = new WebpConversionOptions
        {
            SkipConversion = (_, file) => file.FileName.Contains("skip")
        };

        var middleware = new WebpUploadMiddleware(
            _ => Task.CompletedTask,
            NullLogger<WebpUploadMiddleware>.Instance);

        var skipFile = CreateFormFile("skip-this.jpg", "image/jpeg", new byte[] { 0x01 });

        var context = new DefaultHttpContext();
        context.Request.ContentType = "multipart/form-data; boundary=----test";
        var files = new FormFileCollection { skipFile };
        context.Request.Form = new FormCollection(
            new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>(), files);

        await middleware.InvokeAsync(context, serviceMock.Object, OptionsWrapper(options));

        serviceMock.Verify(
            s => s.ConvertAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task MatchingFile_IsConverted_AndFormReplaced()
    {
        var webpData = new MemoryStream(new byte[] { 0xAA, 0xBB });
        var serviceMock = new Mock<IWebpConversionService>();
        serviceMock
            .Setup(s => s.ConvertAsync(It.IsAny<Stream>(), "photo.jpg", It.IsAny<CancellationToken>()))
            .ReturnsAsync((webpData, "photo.webp"));

        IFormFileCollection? capturedFiles = null;
        var middleware = new WebpUploadMiddleware(
            ctx => { capturedFiles = ctx.Request.Form.Files; return Task.CompletedTask; },
            NullLogger<WebpUploadMiddleware>.Instance);

        var jpgFile = CreateFormFile("photo.jpg", "image/jpeg", new byte[] { 0x01, 0x02, 0x03 });

        var context = new DefaultHttpContext();
        context.Request.ContentType = "multipart/form-data; boundary=----test";
        var files = new FormFileCollection { jpgFile };
        context.Request.Form = new FormCollection(
            new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>(), files);

        await middleware.InvokeAsync(context, serviceMock.Object, OptionsWrapper());

        Assert.NotNull(capturedFiles);
        Assert.Single(capturedFiles);
        Assert.Equal("photo.webp", capturedFiles[0].FileName);
        Assert.Equal("image/webp", capturedFiles[0].ContentType);
    }

    [Fact]
    public async Task ConversionFailure_FallsBackToOriginal()
    {
        var serviceMock = new Mock<IWebpConversionService>();
        serviceMock
            .Setup(s => s.ConvertAsync(It.IsAny<Stream>(), "fail.png", It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("boom"));

        IFormFileCollection? capturedFiles = null;
        var middleware = new WebpUploadMiddleware(
            ctx => { capturedFiles = ctx.Request.Form.Files; return Task.CompletedTask; },
            NullLogger<WebpUploadMiddleware>.Instance);

        var pngFile = CreateFormFile("fail.png", "image/png", new byte[] { 0x01 });

        var context = new DefaultHttpContext();
        context.Request.ContentType = "multipart/form-data; boundary=----test";
        var files = new FormFileCollection { pngFile };
        context.Request.Form = new FormCollection(
            new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>(), files);

        await middleware.InvokeAsync(context, serviceMock.Object, OptionsWrapper());

        Assert.NotNull(capturedFiles);
        Assert.Single(capturedFiles);
        // Falls back to original file
        Assert.Equal("fail.png", capturedFiles[0].FileName);
    }

    [Fact]
    public async Task MixedFiles_OnlyEligibleAreConverted()
    {
        var webpData = new MemoryStream(new byte[] { 0xCC });
        var serviceMock = new Mock<IWebpConversionService>();
        serviceMock
            .Setup(s => s.ConvertAsync(It.IsAny<Stream>(), "photo.jpg", It.IsAny<CancellationToken>()))
            .ReturnsAsync((webpData, "photo.webp"));

        IFormFileCollection? capturedFiles = null;
        var middleware = new WebpUploadMiddleware(
            ctx => { capturedFiles = ctx.Request.Form.Files; return Task.CompletedTask; },
            NullLogger<WebpUploadMiddleware>.Instance);

        var jpgFile = CreateFormFile("photo.jpg", "image/jpeg", new byte[] { 0x01 });
        var pdfFile = CreateFormFile("doc.pdf", "application/pdf", new byte[] { 0x02 });

        var context = new DefaultHttpContext();
        context.Request.ContentType = "multipart/form-data; boundary=----test";
        var files = new FormFileCollection { jpgFile, pdfFile };
        context.Request.Form = new FormCollection(
            new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>(), files);

        await middleware.InvokeAsync(context, serviceMock.Object, OptionsWrapper());

        Assert.NotNull(capturedFiles);
        Assert.Equal(2, capturedFiles.Count);
        Assert.Equal("photo.webp", capturedFiles[0].FileName);
        Assert.Equal("doc.pdf", capturedFiles[1].FileName);
    }

    // ── Helpers ────────────────────────────────────────────────────────

    private static FormFile CreateFormFile(string fileName, string contentType, byte[] data)
    {
        var stream = new MemoryStream(data);
        return new FormFile(stream, 0, data.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };
    }
}

public class WebpConversionServiceTests
{
    [Fact]
    public async Task ConvertAsync_ThrowsOnNullInput()
    {
        var service = CreateService();
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => service.ConvertAsync(null!, "test.jpg"));
    }

    [Fact]
    public async Task ConvertAsync_ThrowsOnEmptyFileName()
    {
        var service = CreateService();
        await Assert.ThrowsAsync<ArgumentException>(
            () => service.ConvertAsync(new MemoryStream(), ""));
    }

    [Fact]
    public async Task ConvertAsync_ThrowsOnWhitespaceFileName()
    {
        var service = CreateService();
        await Assert.ThrowsAsync<ArgumentException>(
            () => service.ConvertAsync(new MemoryStream(), "   "));
    }

    private static WebpConversionService CreateService(WebpConversionOptions? options = null)
    {
        return new WebpConversionService(
            Options.Create(options ?? new WebpConversionOptions()),
            NullLogger<WebpConversionService>.Instance);
    }
}

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddWebpConversion_RegistersServices()
    {
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
        services.AddWebpConversion();
        services.AddLogging();

        var provider = services.BuildServiceProvider();
        var service = provider.GetService<IWebpConversionService>();

        Assert.NotNull(service);
        Assert.IsType<WebpConversionService>(service);
    }

    [Fact]
    public void AddWebpConversion_AppliesConfiguration()
    {
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
        services.AddWebpConversion(opts =>
        {
            opts.QualityFactor = 50;
            opts.Lossless = true;
            opts.MaxInputSizeBytes = 1024;
        });
        services.AddLogging();

        var provider = services.BuildServiceProvider();
        var resolvedOptions = provider.GetRequiredService<IOptions<WebpConversionOptions>>();

        Assert.Equal(50, resolvedOptions.Value.QualityFactor);
        Assert.True(resolvedOptions.Value.Lossless);
        Assert.Equal(1024, resolvedOptions.Value.MaxInputSizeBytes);
    }
}
