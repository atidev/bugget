using System.ComponentModel.DataAnnotations;
namespace Bugget.Entities.DTO.Report;

public sealed class ReportV2CreateDto
{
    [StringLength(120, MinimumLength = 1)]
    public required string Title { get; init; }
}