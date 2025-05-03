using Bugget.Entities.DbModels.Attachment;
using Bugget.Entities.SocketViews;

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
    }
}