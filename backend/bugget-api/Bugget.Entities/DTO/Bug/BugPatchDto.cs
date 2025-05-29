using System.ComponentModel.DataAnnotations;

namespace Bugget.Entities.DTO.Bug;

public sealed class BugPatchDto
{
    [StringLength(512, MinimumLength = 1)]
    public string? Receive { get; init; }
    [StringLength(512, MinimumLength = 1)]
    public string? Expect { get; init; }
    [Range(0, 2)]
    public int? Status { get; init; }
}