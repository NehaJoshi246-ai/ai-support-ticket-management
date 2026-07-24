using Microsoft.EntityFrameworkCore;
using SupportTickets.Api.DTOs.Comments;
using SupportTickets.Domain.Entities;
using SupportTickets.Domain.Exceptions;
using SupportTickets.Infrastructure.Data;

namespace SupportTickets.Api.Services;

public class TicketCommentService
{
    private readonly AppDbContext _db;

    public TicketCommentService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<CommentResponse>> ListForTicketAsync(
        int ticketId,
        CancellationToken cancellationToken = default)
    {
        await EnsureTicketExistsAsync(ticketId, cancellationToken);

        var comments = await _db.Comments
            .AsNoTracking()
            .Include(c => c.CreatedBy)
            .Where(c => c.TicketId == ticketId)
            .OrderBy(c => c.Id)
            .ToListAsync(cancellationToken);

        return comments
            .OrderBy(c => c.CreatedAt)
            .ThenBy(c => c.Id)
            .Select(Map)
            .ToList();
    }

    public async Task<CommentResponse> AddAsync(
        int ticketId,
        CreateCommentRequest request,
        CancellationToken cancellationToken = default)
    {
        await EnsureTicketExistsAsync(ticketId, cancellationToken);

        var authorExists = await _db.Users.AnyAsync(u => u.Id == request.CreatedById, cancellationToken);
        if (!authorExists)
        {
            throw new ValidationException(
                nameof(request.CreatedById),
                $"User with id {request.CreatedById} was not found.");
        }

        var comment = new TicketComment
        {
            TicketId = ticketId,
            Body = request.Body.Trim(),
            CreatedById = request.CreatedById,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _db.Comments.Add(comment);
        await _db.SaveChangesAsync(cancellationToken);

        await _db.Entry(comment).Reference(c => c.CreatedBy).LoadAsync(cancellationToken);
        return Map(comment);
    }

    private async Task EnsureTicketExistsAsync(int ticketId, CancellationToken cancellationToken)
    {
        var exists = await _db.Tickets.AnyAsync(t => t.Id == ticketId, cancellationToken);
        if (!exists)
        {
            throw new NotFoundException($"Ticket with id {ticketId} was not found.");
        }
    }

    private static CommentResponse Map(TicketComment comment) => new()
    {
        Id = comment.Id,
        TicketId = comment.TicketId,
        Body = comment.Body,
        CreatedById = comment.CreatedById,
        CreatedByName = comment.CreatedBy.Name,
        CreatedAt = comment.CreatedAt
    };
}
