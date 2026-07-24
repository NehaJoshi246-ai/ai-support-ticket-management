using System.ComponentModel.DataAnnotations;
using SupportTickets.Domain.Enums;

namespace SupportTickets.Api.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
public sealed class AllowedTicketPriorityAttribute : ValidationAttribute
{
    private static readonly HashSet<string> AllowedNames =
        new(Enum.GetNames<TicketPriority>(), StringComparer.OrdinalIgnoreCase);

    private static readonly string AllowedList = string.Join(", ", Enum.GetNames<TicketPriority>());

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var member = validationContext.MemberName ?? "Priority";

        if (value is null || (value is string s && string.IsNullOrWhiteSpace(s)))
        {
            return new ValidationResult("Priority is required.", new[] { member });
        }

        var raw = (value as string ?? value.ToString())!.Trim();

        // Require named values (Low, Medium, High, Critical) — not bare integers.
        if (!AllowedNames.Contains(raw))
        {
            return new ValidationResult(
                $"Priority must be one of: {AllowedList}.",
                new[] { member });
        }

        return ValidationResult.Success;
    }
}
