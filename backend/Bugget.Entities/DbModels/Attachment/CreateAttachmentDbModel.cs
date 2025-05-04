namespace Bugget.Entities.DbModels.Attachment;

public sealed class CreateAttachmentDbModel
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
    /// Тип хранилища 0=Temp , 1=Standard, 2=Cold
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
}