namespace Bugget.Entities.DTO;

public sealed class AttachmentDto
{
    public required string Path { get; init; }
    public required int AttachType { get; init; }
}