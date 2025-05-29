using Bugget.BO.Interfaces;
using Bugget.DA.Interfaces;
using Bugget.Entities.BO.AttachmentBo;
using Bugget.Entities.DbModels.Attachment;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Bugget.BO.Services.Attachments
{
    public sealed class ImageOptimizeWriter(
        IFileStorageClient fileStorageClient,
        IAttachmentKeyGenerator keyGen,
        IOptions<OptimizatorSettings> opt)
    {
        private readonly WebpEncoder _encoder = new WebpEncoder
        {
            Quality = opt.Value.WebpQuality,
            FileFormat = opt.Value.FileFormat,
            Method = opt.Value.Method,
            TransparentColorMode = opt.Value.TransparentColorMode
        };

        public async Task<OptimizationResult> OptimizeWriteAsync(
            string? organizationId,
            int reportId,
            AttachmentDbModel attachmentDbModel,
            Stream originalStream,
            CancellationToken ct = default)
        {
            // 1) Сброс позиции для Seekable-потоков
            if (originalStream.CanSeek)
                originalStream.Position = 0;

            // 2) Декодируем и ориентируем
            using var image = await Image.LoadAsync<Rgba32>(originalStream, ct);
            image.Mutate(ctx =>
            {
                ctx.AutoOrient();
                if (image.Width > opt.Value.MaxOriginalWidth)
                {
                    ctx.Resize(new ResizeOptions
                    {
                        Size = new Size(opt.Value.MaxOriginalWidth, 0),
                        Mode = ResizeMode.Max
                    });
                }
            });

            // 3) Стрипаем метаданные
            image.Metadata.ExifProfile = null;
            image.Metadata.IccProfile = null;

            // 4) Сохраняем в WebP в память (оригинал)
            var storageKey = keyGen.GetOriginalKey(
                organizationId, reportId, attachmentDbModel.EntityId!.Value, "webp");
            await using var origMs = new MemoryStream();
            await image.SaveAsWebpAsync(origMs, _encoder, ct);
            origMs.Position = 0;

            // 5) Генерим и сохраняем Preview
            using var previewImg = image.Clone(ctx =>
            {
                ctx.AutoOrient();
                ctx.Resize(new ResizeOptions
                {
                    Size = new Size(opt.Value.MaxPreviewSize, opt.Value.MaxPreviewSize),
                    Mode = ResizeMode.Max
                });
            });
            previewImg.Metadata.ExifProfile = null;
            previewImg.Metadata.IccProfile = null;

            var previewKey = keyGen.GetPreviewKey(storageKey);
            await using var prevMs = new MemoryStream();
            await previewImg.SaveAsWebpAsync(prevMs, _encoder, ct);
            prevMs.Position = 0;

            // 6) Параллельно шлём оба потока в хранилище
            var writeOrigTask = fileStorageClient.WriteAsync(storageKey, origMs, ct);
            var writePreviewTask = fileStorageClient.WriteAsync(previewKey, prevMs, ct);
            await Task.WhenAll(writeOrigTask, writePreviewTask);

            // 7) Возвращаем результат
            return new OptimizationResult(
                FileName: Path.ChangeExtension(attachmentDbModel.FileName, ".webp"),
                StorageKey: storageKey,
                MimeType: "image/webp",
                LengthBytes: origMs.Length,
                IsGzipCompressed: false,
                HasPreview: true,
                PreviewLengthBytes: prevMs.Length
            );
        }
    }
}