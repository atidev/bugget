using System.ComponentModel.DataAnnotations;

namespace Bugget.Entities.DTO.Comment;

public sealed class CommentDto
{
    [StringLength(2048, MinimumLength = 1)]
    public required string Text { get; init; }
}
