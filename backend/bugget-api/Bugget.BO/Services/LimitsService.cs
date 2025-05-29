using Bugget.DA.Postgres;
using Bugget.Entities.Authentication;
using Monade;
using Bugget.BO.Errors;
using Bugget.Entities.BO;

namespace Bugget.BO.Services;

public sealed class LimitsService(
    AttachmentDbClient attachmentDbClient)
{
    private const int MaxAttachmentsCount = 10;

    public async Task<Error?> ValidateBugAttachmentLimitAsync(
        UserIdentity user,
        int reportId,
        int bugId,
        AttachType attachType)
    {
        if (attachType == AttachType.Expected || attachType == AttachType.Fact)
        {
            var bugAttachmentsCount = await attachmentDbClient.GetBugAttachmentsCountAsync(
                user.OrganizationId,
                reportId,
                bugId,
                (int)attachType);
            return bugAttachmentsCount < MaxAttachmentsCount
                ? null
                : BoErrors.AttachmentLimitExceeded;
        }
        else
        {
            return BoErrors.AttachmentTypeNotAllowed;
        }
    }

    public async Task<Error?> ValidateCommentAttachmentLimitAsync(
        UserIdentity user,
        int reportId,
        int bugId,
        int commentId)
    {
        var commentAttachmentsCount = await attachmentDbClient.GetCommentAttachmentsCountAsync(
            user.Id,
            user.OrganizationId,
            reportId,
            bugId,
            commentId);

        return commentAttachmentsCount < MaxAttachmentsCount
            ? null
            : BoErrors.AttachmentLimitExceeded;
    }
}