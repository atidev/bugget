using Bugget.BO.Interfaces;
using Bugget.DA.Interfaces;
using Bugget.Entities.BO.AttachmentBo;
using Bugget.Entities.Constants;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;

namespace Bugget.BO.Services.Attachments
{
    public sealed class AttachmentWriterService(
        ImageOptimizator imageWriter,
        IAttachmentKeyGenerator keyGen,
        IFileStorageClient fileStorage,
        TextOptimizator textOptimizator,
        IOptions<OptimizatorSettings> opt)
    {
        private readonly WebpEncoder _encoder = new WebpEncoder
        {
            Quality = opt.Value.WebpQuality,
            FileFormat = opt.Value.FileFormat,
            Method = opt.Value.Method,
            TransparentColorMode = opt.Value.TransparentColorMode
        };

        private static bool IsDevelopment =
            Environment.GetEnvironmentVariable(EnvironmentConstants.AspnetcoreEnvironment)?
                .Equals("development", StringComparison.OrdinalIgnoreCase) ?? false;

        private static readonly string[] CompressibleMimeTypes =
            { "text/plain", "text/csv", "text/xml", "text/yaml", "text/markdown", "text/json", "text/x-markdown" };

        private static readonly string[] ImageMimeTypes =
            { "image/png", "image/jpeg", "image/webp" };

        public async Task<AttachmentSaveResult> ProcessAsync(
            string? organizationId,
            int reportId,
            int entityId,
            Stream originalStream,
            FileMeta fileMeta,
            CancellationToken ct = default)
        {
            var fileMime = IsDevelopment
                ? fileMeta.ContentType
                : MimeHelper.GuessMime(originalStream);

            var ext = Path.GetExtension(fileMeta.FileName).ToLowerInvariant();
            // ── 1) Изображение ─────────────────────
            if (ImageMimeTypes.Contains(fileMime))
            {
                var storageKey = keyGen.GetOriginalKey(organizationId, reportId, entityId, "webp");
                var previewKey = keyGen.GetPreviewKey(storageKey);

                using var img = await imageWriter.OptimizeOriginalAsync(originalStream);

                await using var origMs = new MemoryStream();
                await img.SaveAsWebpAsync(origMs, _encoder, ct);
                origMs.Position = 0;

                var previewImg = imageWriter.GeneratePreviewAsync(img); // clone
                await using var prevMs = new MemoryStream();
                await previewImg.SaveAsWebpAsync(prevMs, _encoder, ct);
                prevMs.Position = 0;

                await Task.WhenAll(
                    fileStorage.WriteAsync(storageKey, origMs, ct),
                    fileStorage.WriteAsync(previewKey, prevMs, ct));

                previewImg.Dispose();

                return new AttachmentSaveResult(
                    StorageKey: storageKey,
                    MimeType: "image/webp",
                    LengthBytes: origMs.Length,
                    IsGzipCompressed: false,
                    HasPreview: true,
                    PreviewLengthBytes: prevMs.Length
                );
            }

            // ── 2) Текстовые форматы (gzip) ────────
            if (CompressibleMimeTypes.Contains(fileMime))
            {
                // компрессия gzip
                var compressedStream =
                    await textOptimizator.CompressAsync(originalStream, ct);
                var storageKey = keyGen.GetOriginalKey(organizationId, reportId, entityId, ext);
                await fileStorage.WriteAsync(storageKey, compressedStream, ct);

                return new AttachmentSaveResult(
                    StorageKey: storageKey,
                    MimeType: fileMime,
                    LengthBytes: compressedStream.Length,
                    IsGzipCompressed: true,
                    HasPreview: false,
                    PreviewLengthBytes: 0
                );
            }

            // ── 3) Всё остальное (байт в байт) ──────
            var key = keyGen.GetOriginalKey(organizationId, reportId, entityId, ext);

            await fileStorage.WriteAsync(key, originalStream, ct);

            // определяем размер
            long length = originalStream.CanSeek
                ? originalStream.Length
                : new FileInfo(key).Length;

            return new AttachmentSaveResult(
                StorageKey: key,
                MimeType: fileMime,
                LengthBytes: length,
                IsGzipCompressed: false,
                HasPreview: false,
                PreviewLengthBytes: 0
            );
        }
    }
}