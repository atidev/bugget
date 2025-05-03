using Bugget.Entities.Options;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Bugget.BO.Services.Attachments;

public sealed class ImageOptimizator
{
    private readonly OptimizatorOptions _opt;
    private readonly WebpEncoder _encoder;

    public ImageOptimizator(IOptions<OptimizatorOptions> opt)
    {
        _opt = opt.Value;
        _encoder = new WebpEncoder
        {
            Quality = _opt.WebpQuality,
            FileFormat = WebpFileFormatType.Lossy
        };
    }

    public async Task<Stream> OptimizeOriginalAsync(
        Stream input,
        CancellationToken ct = default)
    {
        if (input.CanSeek) input.Position = 0;
        using var image = await Image.LoadAsync<Rgba32>(input, ct);
        image.Mutate(x =>
        {
            x.AutoOrient();
            if (image.Width > _opt.MaxOriginalWidth)
                x.Resize(new ResizeOptions
                {
                    Size = new Size(_opt.MaxOriginalWidth, 0),
                    Mode = ResizeMode.Max
                });
        });
        image.Metadata.ExifProfile = null;
        image.Metadata.IccProfile = null;

        var ms = new MemoryStream();
        await image.SaveAsWebpAsync(ms, _encoder, ct);
        ms.Position = 0;
        return ms;
    }

    public async Task<Stream> GeneratePreviewAsync(
        Stream input,
        CancellationToken ct = default)
    {
        if (input.CanSeek) input.Position = 0;
        using var image = await Image.LoadAsync<Rgba32>(input, ct);
        image.Mutate(x =>
        {
            x.AutoOrient();
            x.Resize(new ResizeOptions
            {
                Size = new Size(_opt.MaxPreviewSize, _opt.MaxPreviewSize),
                Mode = ResizeMode.Max
            });
        });
        image.Metadata.ExifProfile = null;
        image.Metadata.IccProfile = null;

        var ms = new MemoryStream();
        await image.SaveAsWebpAsync(ms, _encoder, ct);
        ms.Position = 0;
        return ms;
    }
}