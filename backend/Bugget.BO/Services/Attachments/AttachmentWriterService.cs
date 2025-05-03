using Bugget.BO.Interfaces;
using Bugget.DA.Interfaces;
using Bugget.Entities.BO.AttachmentBo;
using Bugget.Entities.Constants;

namespace Bugget.BO.Services.Attachments
{
    public sealed class AttachmentWriterService(
        ImageOptimizator imageWriter,
        IAttachmentKeyGenerator keyGen,
        IFileStorageClient fileStorage,
        TextOptimizator textOptimizator)
    {
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

                // 1) Получили оптимизированный WebP-поток
                await using var optimizedStream =
                    await imageWriter.OptimizeOriginalAsync(originalStream, ct);
                await fileStorage.WriteAsync(storageKey, optimizedStream, ct);

                if (optimizedStream.CanSeek)
                    optimizedStream.Position = 0;

                // 2) Получили preview-поток
                await using var previewStream =
                    await imageWriter.GeneratePreviewAsync(optimizedStream, ct);
                await fileStorage.WriteAsync(previewKey, previewStream, ct);

                return new AttachmentSaveResult(
                    StorageKey: storageKey,
                    MimeType: "image/webp",
                    LengthBytes: optimizedStream.Length,
                    IsGzipCompressed: false,
                    HasPreview: true,
                    PreviewLengthBytes: previewStream.Length
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