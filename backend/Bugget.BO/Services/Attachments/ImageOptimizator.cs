using Bugget.Entities.BO.AttachmentBo;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Bugget.BO.Services.Attachments;

public sealed class ImageOptimizator(IOptions<OptimizatorSettings> opt)
{
    private readonly OptimizatorSettings _opt = opt.Value;

    public async Task<Image<Rgba32>> OptimizeOriginalAsync(
        Stream input)
    {
        if (input.CanSeek) input.Position = 0;
        var image = await Image.LoadAsync<Rgba32>(input);
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
        return image;
    }

    public Image<Rgba32> GeneratePreviewAsync(Image<Rgba32> source)
    {
        var preview = source.Clone(ctx =>
        {
            ctx.Resize(new ResizeOptions {
                Size = new Size(_opt.MaxPreviewSize,_opt.MaxPreviewSize),
                Mode = ResizeMode.Max
            });
            ctx.AutoOrient();
        });
        preview.Metadata.ExifProfile = null;
        preview.Metadata.IccProfile  = null;
        return preview;
    }
}