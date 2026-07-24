using SupportTickets.Domain.Enums;

namespace SupportTickets.Api.Validation;

public static class TicketPriorityParser
{
    public static TicketPriority Parse(string priority) =>
        Enum.Parse<TicketPriority>(priority.Trim(), ignoreCase: true);
}
