using System.ComponentModel.DataAnnotations;
using SupportTickets.Api.Validation;
using SupportTickets.Domain.Enums;

namespace SupportTickets.Api.DTOs.Tickets;

public class TransitionStatusRequest
{
    [Required(ErrorMessage = "Status is required.")]
    [AllowedTicketStatus]
    public string Status { get; set; } = string.Empty;
}
