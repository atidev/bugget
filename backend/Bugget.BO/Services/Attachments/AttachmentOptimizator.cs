using Bugget.BO.Mappers;
using Bugget.DA.Interfaces;
using Bugget.DA.Postgres;
using Bugget.DA.WebSockets;
using Bugget.Entities.BO.AttachmentBo;
using Bugget.Entities.Constants;
using Bugget.Entities.DbModels.Attachment;
using Microsoft.Extensions.Logging;

namespace Bugget.BO.Services.Attachments
{
    public sealed class AttachmentOptimizator(
        IFileStorageClient fileStorage,
        TextOptimizeWriter textOptimizator,
        ImageOptimizeWriter imageOptimizatorWriter,
        AttachmentDbClient attachmentDbClient,
        ILogger<AttachmentOptimizator> logger,
        IReportPageHubClient reportPageHubClient
        )
    {

        public static bool CanOptimize(string mimeType) =>
            AttachmentConstants.CompressibleMimeTypes.Contains(mimeType, StringComparer.OrdinalIgnoreCase) ||
            AttachmentConstants.ImageMimeTypes.Contains(mimeType, StringComparer.OrdinalIgnoreCase);

        public async Task OptimizeAttachmentAsync(
            string? organizationId,
            int reportId,
            AttachmentDbModel fromAttachmentDbModel)
        {
            if (fromAttachmentDbModel.StorageKind != (int)StorageKind.Temp)
                return;

            var fileStream = await fileStorage.ReadAsync(fromAttachmentDbModel.StorageKey);

            // Создаем новую оптимизированную версию файла
            OptimizationResult optimizationResult;
            if (AttachmentConstants.ImageMimeTypes.Contains(fromAttachmentDbModel.MimeType))
            {
                optimizationResult = await imageOptimizatorWriter.OptimizeWriteAsync(
                    organizationId,
                    reportId,
                    fromAttachmentDbModel,
                    fileStream
                );
            }
            else if (AttachmentConstants.CompressibleMimeTypes.Contains(fromAttachmentDbModel.MimeType))
            {
                optimizationResult = await textOptimizator.OptimizeWriteAsync(
                    organizationId,
                    reportId,
                    fromAttachmentDbModel,
                    fileStream
                );
            }
            else
            {
                throw new InvalidOperationException("Unsupported mime type");
            }

            logger.LogInformation("Attachment saved: {@FileName}, compress score {@from}-{@to}-{@preview} percent {@percent}%",
        fromAttachmentDbModel.FileName, fileStream.Length, optimizationResult.LengthBytes, optimizationResult.PreviewLengthBytes, (1 - (double)optimizationResult.LengthBytes / fileStream.Length) * 100);

            // Обновляем модель в БД
            var toAttachmentDbModel = await attachmentDbClient.UpdateAttachmentAsync(new UpdateAttachmentDbModel
            {
                Id = fromAttachmentDbModel.Id,
                StorageKey = optimizationResult.StorageKey,
                StorageKind = (int)StorageKind.Standard,
                LengthBytes = optimizationResult.LengthBytes + optimizationResult.PreviewLengthBytes,
                FileName = optimizationResult.FileName,
                MimeType = optimizationResult.MimeType,
                HasPreview = optimizationResult.HasPreview,
                IsGzipCompressed = optimizationResult.IsGzipCompressed,
            });

            // Уведомляем клиентов
            await reportPageHubClient.SendAttachmentOptimizedAsync(reportId, toAttachmentDbModel.ToSocketView());

            // Удаляем старый файл
            await fileStorage.DeleteAsync(fromAttachmentDbModel.StorageKey);
        }
    }
}