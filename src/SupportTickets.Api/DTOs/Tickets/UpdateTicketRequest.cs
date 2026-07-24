using System.ComponentModel.DataAnnotations;
using SupportTickets.Api.Validation;
using SupportTickets.Domain.Enums;

namespace SupportTickets.Api.DTOs.Tickets;

public class UpdateTicketRequest : IValidatableObject
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

    [Range(1, int.MaxValue, ErrorMessage = "AssignedToId must be a valid user id.")]
    public int? AssignedToId { get; set; }

    /// <summary>
    /// Not updatable via PUT. If present, validation returns a field-level 400.
    /// </summary>
    public TicketStatus? Status { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Status.HasValue)
        {
            yield return new ValidationResult(
                "Status cannot be updated via PUT. Use PATCH /api/tickets/{id}/status instead.",
                new[] { nameof(Status) });
        }
    }
}
