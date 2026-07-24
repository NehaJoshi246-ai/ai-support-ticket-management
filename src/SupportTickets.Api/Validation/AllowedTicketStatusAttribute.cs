using System.ComponentModel.DataAnnotations;
using SupportTickets.Domain.Enums;

namespace SupportTickets.Api.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
public sealed class AllowedTicketStatusAttribute : ValidationAttribute
{
    private static readonly HashSet<string> AllowedNames =
        new(Enum.GetNames<TicketStatus>(), StringComparer.OrdinalIgnoreCase);

    private static readonly string AllowedList = string.Join(", ", Enum.GetNames<TicketStatus>());

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var member = validationContext.MemberName ?? "Status";

        if (value is null || (value is string s && string.IsNullOrWhiteSpace(s)))
        {
            return new ValidationResult("Status is required.", new[] { member });
        }

        var raw = (value as string ?? value.ToString())!.Trim();
        if (!AllowedNames.Contains(raw))
        {
            return new ValidationResult(
                $"Status must be one of: {AllowedList}.",
                new[] { member });
        }

        return ValidationResult.Success;
    }
}
