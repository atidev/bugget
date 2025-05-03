namespace Bugget.Entities.DbModels.Attachment;

public sealed class AttachmentCreateDbModel
{
    /// <summary>
    /// Идентификатор сущности ожидаю, получаю, коммент
    /// </summary>
    public required int EntityId { get; init; }

    /// <summary>
    /// Тип сущности ожидаю, получаю, коммент
    /// </summary>
    public required int AttachType { get; init; }


    /// <summary>
    /// Относительный путь либо S3‑key
    /// </summary>
    public required string StorageKey { get; init; }

    /// <summary>
    /// Тип хранилища 0=Local, 1=S3Standard, 2=S3Cold
    /// </summary>
    public required int StorageKind { get; init; }

    /// <summary>
    /// Создатель вложения
    /// </summary>
    public required string CreatorUserId { get; init; }

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