namespace Bugget.Entities.Views.Attachment;

public sealed class AttachmentView
{
    public required int Id { get; init; }
    
    /// <summary>
    /// Идентификатор сущности к которой прикреплен файл
    /// </summary>
    public required int EntityId { get; init; }
    
    /// <summary>
    /// Тип сущности к которой прикреплен файл
    /// </summary>
    public required int AttachType { get; init; }
    
    /// <summary>
    /// Дата создания вложения
    /// </summary>
    public required DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    /// Создатель вложения
    /// </summary>
    public required string CreatorUserId { get; init; }

    /// <summary>
    /// Имя вложения
    /// </summary>
    public required string FileName { get; init; }

    /// <summary>
    /// Есть ли превью
    /// </summary>
    public required bool HasPreview { get; init; }
}