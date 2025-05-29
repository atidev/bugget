namespace Bugget.Entities.SocketViews;

public class AttachmentSocketView
{
    public required int Id { get; init; }

    /// <summary>
    /// Идентификатор сущности ожидаю, получаю, коммент
    /// </summary>
    public required int EntityId { get; init; }

    /// <summary>
    /// Тип сущности ожидаю, получаю, коммент
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