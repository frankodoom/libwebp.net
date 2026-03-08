using Microsoft.AspNetCore.Http;

namespace Libwebp.Net.Middleware;

/// <summary>
/// An <see cref="IFormFile"/> backed by an in-memory <see cref="MemoryStream"/>,
/// used to replace the original upload with the converted WebP data.
/// </summary>
internal sealed class WebpFormFile : IFormFile
{
    private readonly MemoryStream _data;

    public WebpFormFile(MemoryStream data, string name, string fileName)
    {
        _data = data ?? throw new ArgumentNullException(nameof(data));
        Name = name;
        FileName = fileName;
        Length = data.Length;
        ContentType = "image/webp";
        ContentDisposition = $"form-data; name=\"{name}\"; filename=\"{fileName}\"";
    }

    public string ContentDisposition { get; }
    public string ContentType { get; }
    public string FileName { get; }
    public IHeaderDictionary Headers { get; set; } = new HeaderDictionary();
    public long Length { get; }
    public string Name { get; }

    public void CopyTo(Stream target)
    {
        _data.Position = 0;
        _data.CopyTo(target);
    }

    public async Task CopyToAsync(Stream target, CancellationToken cancellationToken = default)
    {
        _data.Position = 0;
        await _data.CopyToAsync(target, cancellationToken);
    }

    public Stream OpenReadStream()
    {
        _data.Position = 0;
        return _data;
    }
}
