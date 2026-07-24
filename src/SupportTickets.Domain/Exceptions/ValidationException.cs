namespace SupportTickets.Domain.Exceptions;

/// <summary>
/// Field-level validation failure that maps to HTTP 400 with an errors dictionary.
/// </summary>
public class ValidationException : Exception
{
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    public ValidationException(string field, string message)
        : base(message)
    {
        Errors = new Dictionary<string, string[]>
        {
            [field] = new[] { message }
        };
    }

    public ValidationException(IReadOnlyDictionary<string, string[]> errors)
        : base("One or more validation errors occurred.")
    {
        Errors = errors;
    }
}
