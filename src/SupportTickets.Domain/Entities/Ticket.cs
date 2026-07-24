using SupportTickets.Domain.Enums;

namespace SupportTickets.Domain.Entities;

public class Ticket
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TicketPriority Priority { get; set; }
    public TicketStatus Status { get; set; } = TicketStatus.Open;
    public int? AssignedToId { get; set; }
    public int CreatedById { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public User? AssignedTo { get; set; }
    public User CreatedBy { get; set; } = null!;
    public ICollection<TicketComment> Comments { get; set; } = new List<TicketComment>();
}
