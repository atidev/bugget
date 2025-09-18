using System.ComponentModel.DataAnnotations;

namespace Bugget.Entities.DTO.Bug;

public sealed class BugDto
{
    [StringLength(2048, MinimumLength = 1)]
    public string? Receive { get; init; }
    [StringLength(2048, MinimumLength = 1)]
    public string? Expect { get; init; }
}
