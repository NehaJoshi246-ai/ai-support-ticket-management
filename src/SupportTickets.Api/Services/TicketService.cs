using Microsoft.EntityFrameworkCore;
using SupportTickets.Api.DTOs.Tickets;
using SupportTickets.Api.Validation;
using SupportTickets.Domain.Entities;
using SupportTickets.Domain.Enums;
using SupportTickets.Domain.Exceptions;
using SupportTickets.Infrastructure.Data;

namespace SupportTickets.Api.Services;

public class TicketService
{
    private readonly AppDbContext _db;

    public TicketService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<TicketResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        // SQLite cannot ORDER BY DateTimeOffset in SQL — order by Id (identity) as newest-first proxy,
        // then refine by CreatedAt in memory.
        var tickets = await _db.Tickets
            .AsNoTracking()
            .Include(t => t.CreatedBy)
            .Include(t => t.AssignedTo)
            .OrderByDescending(t => t.Id)
            .ToListAsync(cancellationToken);

        return tickets
            .OrderByDescending(t => t.CreatedAt)
            .Select(Map)
            .ToList();
    }

    public async Task<TicketResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var ticket = await _db.Tickets
            .AsNoTracking()
            .Include(t => t.CreatedBy)
            .Include(t => t.AssignedTo)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        return ticket is null ? null : Map(ticket);
    }

    public async Task<TicketResponse> CreateAsync(CreateTicketRequest request, CancellationToken cancellationToken = default)
    {
        await EnsureUserExistsAsync(request.CreatedById, nameof(request.CreatedById), cancellationToken);

        if (request.AssignedToId is int assigneeId)
        {
            await EnsureUserExistsAsync(assigneeId, nameof(request.AssignedToId), cancellationToken);
        }

        var ticket = new Ticket
        {
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            Priority = TicketPriorityParser.Parse(request.Priority),
            Status = TicketStatus.Open,
            CreatedById = request.CreatedById,
            AssignedToId = request.AssignedToId,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _db.Tickets.Add(ticket);
        await _db.SaveChangesAsync(cancellationToken);

        return (await GetByIdAsync(ticket.Id, cancellationToken))!;
    }

    public async Task<TicketResponse> UpdateAsync(int id, UpdateTicketRequest request, CancellationToken cancellationToken = default)
    {
        var ticket = await _db.Tickets
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken)
            ?? throw new NotFoundException($"Ticket with id {id} was not found.");

        if (request.AssignedToId is int assigneeId)
        {
            await EnsureUserExistsAsync(assigneeId, nameof(request.AssignedToId), cancellationToken);
        }

        ticket.Title = request.Title.Trim();
        ticket.Description = request.Description.Trim();
        ticket.Priority = TicketPriorityParser.Parse(request.Priority);
        ticket.AssignedToId = request.AssignedToId;

        await _db.SaveChangesAsync(cancellationToken);

        return (await GetByIdAsync(ticket.Id, cancellationToken))!;
    }

    private async Task EnsureUserExistsAsync(int userId, string fieldName, CancellationToken cancellationToken)
    {
        var exists = await _db.Users.AnyAsync(u => u.Id == userId, cancellationToken);
        if (!exists)
        {
            throw new ValidationException(fieldName, $"User with id {userId} was not found.");
        }
    }

    private static TicketResponse Map(Ticket ticket) => new()
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
