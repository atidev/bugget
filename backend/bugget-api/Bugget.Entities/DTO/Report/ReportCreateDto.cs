using System.ComponentModel.DataAnnotations;
namespace Bugget.Entities.DTO.Report;

public sealed class ReportCreateDto
{
    [StringLength(128, MinimumLength = 1)]
    public required string Title { get; init; }
}