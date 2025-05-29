namespace Bugget.Entities.DTO.Bug;

public sealed class BugUpdateDtoObsolete
{
    public string? Receive { get; init; }
    public string? Expect { get; init; }
    public int? Status { get; init; }
}