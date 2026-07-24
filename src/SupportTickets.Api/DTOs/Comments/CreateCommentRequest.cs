using System.ComponentModel.DataAnnotations;

namespace SupportTickets.Api.DTOs.Comments;

public class CreateCommentRequest
{
    [Required(AllowEmptyStrings = false, ErrorMessage = "Comment message is required.")]
    [MaxLength(4000, ErrorMessage = "Comment message must be at most 4000 characters.")]
    public string Body { get; set; } = string.Empty;

    [Required(ErrorMessage = "CreatedById is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "CreatedById must be a valid user id.")]
    public int CreatedById { get; set; }
}
