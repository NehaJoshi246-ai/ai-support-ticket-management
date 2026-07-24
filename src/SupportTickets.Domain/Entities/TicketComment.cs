namespace SupportTickets.Domain.Entities;

public class TicketComment
{
    public int Id { get; set; }
    public int TicketId { get; set; }
    public string Body { get; set; } = string.Empty;
    public int CreatedById { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public Ticket Ticket { get; set; } = null!;
    public User CreatedBy { get; set; } = null!;
}
