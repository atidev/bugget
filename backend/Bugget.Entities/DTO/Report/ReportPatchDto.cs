using System.ComponentModel.DataAnnotations;
namespace Bugget.Entities.DTO.Report;

public sealed class ReportPatchDto
{
    [StringLength(120, MinimumLength = 1)]
    public string? Title { get; init; }
    [Range(0, 3)]
    public int? Status { get; init; }
    [StringLength(256, MinimumLength = 1)]
    public string? ResponsibleUserId { get; init; }
}