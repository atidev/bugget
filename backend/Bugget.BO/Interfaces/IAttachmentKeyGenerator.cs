using Bugget.Entities.BO;

namespace Bugget.BO.Interfaces
{
    public interface IAttachmentKeyGenerator
    {
        string GetOriginalKey(string? organizationId, int reportId, int entityId, string extension);
        string GetPreviewKey(string originalKey);
    }
}