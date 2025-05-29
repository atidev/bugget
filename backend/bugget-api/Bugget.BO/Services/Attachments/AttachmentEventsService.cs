using Bugget.BO.Interfaces;
using Bugget.BO.Mappers;
using Bugget.DA.Interfaces;
using Bugget.DA.WebSockets;
using Bugget.Entities.Authentication;
using Bugget.Entities.DbModels.Attachment;

namespace Bugget.BO.Services.Attachments
{
    public class AttachmentEventsService(
        IReportPageHubClient reportPageHubClient,
        IFileStorageClient fileStorageClient,
        IAttachmentKeyGenerator keyGen,
        AttachmentOptimizator attachmentOptimizator
            )
    {
        public async Task HandleAttachmentCreateEventAsync(int reportId, UserIdentity user, AttachmentDbModel attachmentDbModel)
        {
            await Task.WhenAll(
                reportPageHubClient.SendAttachmentCreateAsync(reportId, attachmentDbModel.ToSocketView(), user.SignalRConnectionId),
                attachmentOptimizator.OptimizeAttachmentAsync(user.OrganizationId, reportId, attachmentDbModel)
        );
        }

        public async Task HandleAttachmentDeleteEventAsync(int reportId, UserIdentity user, AttachmentDbModel attachmentDbModel)
        {
            await Task.WhenAll(
                reportPageHubClient.SendAttachmentDeleteAsync(reportId, attachmentDbModel.Id, attachmentDbModel.EntityId.Value, attachmentDbModel.AttachType, user.SignalRConnectionId),
                fileStorageClient.DeleteAsync(attachmentDbModel.StorageKey),
                attachmentDbModel.HasPreview.Value ? fileStorageClient.DeleteAsync(keyGen.GetPreviewKey(attachmentDbModel.StorageKey)) : Task.CompletedTask
            );
        }
    }
}