using System.ComponentModel.DataAnnotations;
using SupportTickets.Api.Validation;

namespace SupportTickets.Api.DTOs.Tickets;

public class CreateTicketRequest
{
    [Required(AllowEmptyStrings = false, ErrorMessage = "Title is required.")]
    [MaxLength(200, ErrorMessage = "Title must be at most 200 characters.")]
    public string Title { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = false, ErrorMessage = "Description is required.")]
    [MaxLength(4000, ErrorMessage = "Description must be at most 4000 characters.")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Priority is required.")]
    [AllowedTicketPriority]
    public string Priority { get; set; } = string.Empty;

    [Required(ErrorMessage = "CreatedById is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "CreatedById must be a valid user id.")]
    public int CreatedById { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "AssignedToId must be a valid user id.")]
    public int? AssignedToId { get; set; }
}
