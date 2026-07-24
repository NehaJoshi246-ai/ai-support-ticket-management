using SupportTickets.Domain.Enums;
using SupportTickets.Domain.Rules;

namespace SupportTickets.Domain.Exceptions;

/// <summary>
/// Thrown when a ticket status change is not allowed by <see cref="TransitionMap"/>.
/// </summary>
public class InvalidTransitionException : Exception
{
    public TicketStatus FromStatus { get; }
    public TicketStatus ToStatus { get; }
    public IReadOnlyList<TicketStatus> AllowedNextStatuses { get; }

    public InvalidTransitionException(TicketStatus fromStatus, TicketStatus toStatus)
        : base(BuildMessage(fromStatus, toStatus, TransitionMap.GetAllowedNext(fromStatus)))
    {
        FromStatus = fromStatus;
        ToStatus = toStatus;
        AllowedNextStatuses = TransitionMap.GetAllowedNext(fromStatus);
    }

    private static string BuildMessage(
        TicketStatus from,
        TicketStatus to,
        IReadOnlyList<TicketStatus> allowedNext)
    {
        if (allowedNext.Count == 0)
        {
            return $"Cannot transition from {from} to {to}. {from} is a terminal status; no further transitions are allowed.";
        }

        var allowed = string.Join(", ", allowedNext);
        return $"Cannot transition from {from} to {to}. Allowed next statuses from {from}: {allowed}.";
    }
}
