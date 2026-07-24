namespace SupportTickets.Api.DTOs.Comments;

public class CommentResponse
{
    public int Id { get; set; }
    public int TicketId { get; set; }
    public string Body { get; set; } = string.Empty;
    public int CreatedById { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
}
