using System.Buffers;
using System.IO.Compression;
using Bugget.BO.Interfaces;
using Bugget.DA.Interfaces;
using Bugget.Entities.BO.AttachmentBo;
using Bugget.Entities.DbModels.Attachment;

namespace Bugget.BO.Services.Attachments
{
    public sealed class TextOptimizeWriter(
        IFileStorageClient fileStorage,
        IAttachmentKeyGenerator keyGen
    )
    {
        private const int BufferSize = 128 * 1024; // 128 KiB из пула

        public async Task<OptimizationResult> OptimizeWriteAsync(
            string? organizationId,
            int reportId,
            AttachmentDbModel attachmentDbModel,
            Stream originalStream,
            CancellationToken ct = default)
        {
            // 1) Сброс позиции, если возможно
            if (originalStream.CanSeek)
                originalStream.Position = 0;

            // 2) Готовим ключ в хранилище
            var ext = Path.GetExtension(attachmentDbModel.FileName);
            var storageKey = keyGen.GetOriginalKey(
                organizationId,
                reportId,
                attachmentDbModel.EntityId!.Value,
                ext);

            // 3) Сжимаем в память, читая из пула
            await using var compressedMs = new MemoryStream();
            await using (var gzip = new GZipStream(
                compressedMs,
                CompressionLevel.Optimal,
                leaveOpen: true))
            {
                var buffer = ArrayPool<byte>.Shared.Rent(BufferSize);
                try
                {
                    int read;
                    while ((read = await originalStream.ReadAsync(buffer, 0, buffer.Length, ct)) > 0)
                    {
                        await gzip.WriteAsync(buffer, 0, read, ct);
                    }
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(buffer);
                }
            }
            compressedMs.Position = 0;

            // 4) Пишем в файловое хранилище
            await fileStorage.WriteAsync(storageKey, compressedMs, ct);

            // 5) Возвращаем информацию о результате
            return new OptimizationResult(
                FileName: attachmentDbModel.FileName,
                StorageKey: storageKey,
                MimeType: attachmentDbModel.MimeType,
                LengthBytes: compressedMs.Length,
                IsGzipCompressed: true,
                HasPreview: false,
                PreviewLengthBytes: 0);
        }
    }
}