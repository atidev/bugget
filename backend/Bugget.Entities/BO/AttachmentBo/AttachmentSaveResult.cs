namespace Bugget.Entities.BO.AttachmentBo
{
    public record AttachmentSaveResult(
        string StorageKey,
        string MimeType,
        long LengthBytes,
        bool IsGzipCompressed,
        bool HasPreview,
        long PreviewLengthBytes
    );
}