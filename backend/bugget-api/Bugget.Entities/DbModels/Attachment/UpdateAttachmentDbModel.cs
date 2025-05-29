namespace Bugget.Entities.DbModels.Attachment;

public sealed class UpdateAttachmentDbModel
{
    public required int Id { get; init; }

    /// <summary>
    /// Относительный путь либо S3‑key
    /// </summary>
    public required string StorageKey { get; init; }

    /// <summary>
    /// Тип хранилища 0=Temp , 1=Standard, 2=Cold
    /// </summary>
    public required int StorageKind { get; init; }

    /// <summary>
    /// Размер вложения
    /// </summary>
    public required long LengthBytes { get; init; }

    /// <summary>
    /// Имя вложения
    /// </summary>
    public required string FileName { get; init; }

    /// <summary>
    /// Тип вложения
    /// </summary>
    public required string MimeType { get; init; }

    /// <summary>
    /// Есть ли превью
    /// </summary>
    public required bool HasPreview { get; init; }

    /// <summary>
    /// Сжато ли вложение gzip
    /// </summary>
    public required bool IsGzipCompressed { get; init; }
}