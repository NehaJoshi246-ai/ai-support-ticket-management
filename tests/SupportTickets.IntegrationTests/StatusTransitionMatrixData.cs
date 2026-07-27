using SupportTickets.Domain.Enums;
using SupportTickets.Domain.Rules;

namespace SupportTickets.IntegrationTests;

/// <summary>
/// All 25 (fromStatus, toStatus) pairs and expected HTTP outcomes.
/// </summary>
public static class StatusTransitionMatrixData
{
    public static IEnumerable<object[]> AllPairs()
    {
        foreach (var from in Enum.GetValues<TicketStatus>())
        {
            foreach (var to in Enum.GetValues<TicketStatus>())
            {
                var shouldSucceed = from == to || TransitionMap.CanTransition(from, to);
                yield return new object[] { from, to, shouldSucceed };
            }
        }
    }

    public static string Format(TicketStatus from, TicketStatus to) =>
        $"{from} → {to}";
}
