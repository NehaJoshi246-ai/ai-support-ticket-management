using SupportTickets.Domain.Enums;

namespace SupportTickets.Domain.Rules;

/// <summary>
/// Explicit allow-list of ticket status transitions.
/// Anything not listed here is rejected by the transition service.
/// </summary>
public static class TransitionMap
{
    public static readonly IReadOnlyDictionary<TicketStatus, IReadOnlyList<TicketStatus>> Allowed =
        new Dictionary<TicketStatus, IReadOnlyList<TicketStatus>>
        {
            [TicketStatus.Open] = new[] { TicketStatus.InProgress, TicketStatus.Cancelled },
            [TicketStatus.InProgress] = new[] { TicketStatus.Resolved, TicketStatus.Cancelled },
            [TicketStatus.Resolved] = new[] { TicketStatus.Closed },
            [TicketStatus.Closed] = Array.Empty<TicketStatus>(),
            [TicketStatus.Cancelled] = Array.Empty<TicketStatus>()
        };

    public static bool CanTransition(TicketStatus from, TicketStatus to) =>
        GetAllowedNext(from).Contains(to);

    public static IReadOnlyList<TicketStatus> GetAllowedNext(TicketStatus from) =>
        Allowed.TryGetValue(from, out var next) ? next : Array.Empty<TicketStatus>();
}
