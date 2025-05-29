using Bugget.Entities.DbModels.Attachment;
using Bugget.Entities.SocketViews;
using Bugget.Entities.Views.Attachment;

namespace Bugget.BO.Mappers
{
    public static class AttachmentMapper
    {
        public static AttachmentSocketView ToSocketView(this AttachmentDbModel attachmentDbModel)
        {
            return new AttachmentSocketView
            {
                Id = attachmentDbModel.Id,
                EntityId = attachmentDbModel.EntityId.Value,
                AttachType = attachmentDbModel.AttachType,
                CreatedAt = attachmentDbModel.CreatedAt,
                CreatorUserId = attachmentDbModel.CreatorUserId,
                FileName = attachmentDbModel.FileName,
                HasPreview = attachmentDbModel.HasPreview.Value,
            };
        }

        public static AttachmentView ToView(this AttachmentDbModel attachmentDbModel, int reportId)
        {
            return new AttachmentView
            {
                Id = attachmentDbModel.Id,
                ReportId = reportId,
                BugId = attachmentDbModel.BugId,
                EntityId = attachmentDbModel.EntityId!.Value,
                AttachType = attachmentDbModel.AttachType,
                CreatedAt = attachmentDbModel.CreatedAt,
                CreatorUserId = attachmentDbModel.CreatorUserId ?? string.Empty,
                FileName = attachmentDbModel.FileName ?? string.Empty,
                HasPreview = attachmentDbModel.HasPreview.Value,
            };
        }
    }
}