using Bugget.Entities.BO;
using Bugget.Entities.DbModels;

namespace Bugget.Entities.Mappers;

public static class AttachmentMapper
{
    public static Attachment ToAttachment(this AttachmentDbModel attachment)
    {
        return new Attachment
        {
            Id = attachment.Id,
            BugId = attachment.BugId,
            Path = attachment.Path,
            CreatedAt = attachment.CreatedAt,
            AttachType = attachment.AttachType
        };
    }
} 