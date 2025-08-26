namespace Bugget.Entities.DbModels.Attachment;

public sealed class AttachmentDbModel
{
    public required int Id { get; init; }
    
    private readonly int? _entityId;

    /// <summary>
    /// Идентификатор сущности к которой прикреплен файл
    /// </summary>
    public int? EntityId 
    {
        get => _entityId ?? BugId;
        init => _entityId = value;
    }

    /// <summary>
    /// Тип сущности к которой прикреплен файл
    /// </summary>
    public required int AttachType { get; init; }
    
    /// <summary>
    /// Идентификатор бага
    /// </summary>
    [Obsolete]
    public int BugId { get; init; }

    private readonly string? _storageKey;
    
    /// <summary>
    /// Относительный путь либо S3‑key
    /// </summary>
    public string? StorageKey 
    { 
        get => _storageKey ?? Path; 
        init => _storageKey = value; 
    }

    /// <summary>
    /// Относительный путь
    /// </summary>
    [Obsolete]
    public required string Path { get; init; }

    /// <summary>
    /// Тип хранилища 0=Temp , 1=Standard, 2=Cold
    /// </summary>
    public int? StorageKind { get; init; } = 0;

    /// <summary>
    /// Дата создания вложения
    /// </summary>
    public required DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    /// Создатель вложения
    /// </summary>
    public string CreatorUserId { get; init; } = "obsolete";

    /// <summary>
    /// Размер вложения
    /// </summary>
    public long? LengthBytes { get; init; }

    /// <summary>
    /// Имя вложения
    /// </summary>
    public string FileName { get; init; } = "obsolete.jpg";

    /// <summary>
    /// Тип вложения
    /// </summary>
    public string MimeType { get; init; } = "image/webp";

    /// <summary>
    /// Есть ли превью
    /// </summary>
    public bool? HasPreview { get; init; } = false;

    /// <summary>
    /// Сжато ли вложение gzip
    /// </summary>
    public bool? IsGzipCompressed { get; init; } = false;
}