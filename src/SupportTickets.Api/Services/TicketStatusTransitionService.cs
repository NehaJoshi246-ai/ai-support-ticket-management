using Microsoft.EntityFrameworkCore;
using SupportTickets.Api.DTOs.Tickets;
using SupportTickets.Domain.Enums;
using SupportTickets.Domain.Exceptions;
using SupportTickets.Domain.Rules;
using SupportTickets.Infrastructure.Data;

namespace SupportTickets.Api.Services;

public class TicketStatusTransitionService
{
    private readonly AppDbContext _db;

    public TicketStatusTransitionService(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Applies a status change only if <see cref="TransitionMap"/> allows
    /// current → target. Otherwise throws <see cref="InvalidTransitionException"/>.
    /// </summary>
    public async Task<TicketResponse> TransitionAsync(
        int ticketId,
        TransitionStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        var ticket = await _db.Tickets
            .FirstOrDefaultAsync(t => t.Id == ticketId, cancellationToken)
            ?? throw new NotFoundException($"Ticket with id {ticketId} was not found.");

        var target = Enum.Parse<TicketStatus>(request.Status.Trim(), ignoreCase: true);
        var current = ticket.Status;

        if (current == target)
        {
            // Idempotent no-op: already in the requested status.
            return await LoadResponseAsync(ticketId, cancellationToken);
        }

        if (!TransitionMap.CanTransition(current, target))
        {
            throw new InvalidTransitionException(current, target);
        }

        ticket.Status = target;
        await _db.SaveChangesAsync(cancellationToken);

        return await LoadResponseAsync(ticketId, cancellationToken);
    }

    private async Task<TicketResponse> LoadResponseAsync(int id, CancellationToken cancellationToken)
    {
        var ticket = await _db.Tickets
            .AsNoTracking()
            .Include(t => t.CreatedBy)
            .Include(t => t.AssignedTo)
            .FirstAsync(t => t.Id == id, cancellationToken);

        return new TicketResponse
        {
            Id = ticket.Id,
            Title = ticket.Title,
            Description = ticket.Description,
            Priority = ticket.Priority,
            Status = ticket.Status,
            AssignedToId = ticket.AssignedToId,
            AssignedToName = ticket.AssignedTo?.Name,
            CreatedById = ticket.CreatedById,
            CreatedByName = ticket.CreatedBy.Name,
            CreatedAt = ticket.CreatedAt
        };
    }
}
