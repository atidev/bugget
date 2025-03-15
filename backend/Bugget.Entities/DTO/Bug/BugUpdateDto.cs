namespace Bugget.Entities.DTO.Bug;

public sealed class BugUpdateDto
{
    public string? Receive { get; init; }
    public string? Expect { get; init; }
    public int? Status { get; init; }
}