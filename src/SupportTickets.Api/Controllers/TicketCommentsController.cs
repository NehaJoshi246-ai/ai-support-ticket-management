using Microsoft.AspNetCore.Mvc;
using SupportTickets.Api.DTOs.Comments;
using SupportTickets.Api.Services;
using SupportTickets.Domain.Exceptions;

namespace SupportTickets.Api.Controllers;

[ApiController]
[Route("api/tickets/{ticketId:int}/comments")]
[Produces("application/json")]
public class TicketCommentsController : ControllerBase
{
    private readonly TicketCommentService _comments;

    public TicketCommentsController(TicketCommentService comments)
    {
        _comments = comments;
    }

    /// <summary>List comments for a ticket (oldest first).</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CommentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<CommentResponse>>> List(
        int ticketId,
        CancellationToken cancellationToken)
    {
        try
        {
            var comments = await _comments.ListForTicketAsync(ticketId, cancellationToken);
            return Ok(comments);
        }
        catch (NotFoundException ex)
        {
            return TicketNotFound(ex);
        }
    }

    /// <summary>Add a comment to a ticket.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(CommentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CommentResponse>> Create(
        int ticketId,
        [FromBody] CreateCommentRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var created = await _comments.AddAsync(ticketId, request, cancellationToken);
            return CreatedAtAction(nameof(List), new { ticketId }, created);
        }
        catch (NotFoundException ex)
        {
            return TicketNotFound(ex);
        }
        catch (ValidationException ex)
        {
            foreach (var pair in ex.Errors)
            {
                foreach (var message in pair.Value)
                {
                    ModelState.AddModelError(pair.Key, message);
                }
            }

            return ValidationProblem(ModelState);
        }
    }

    private NotFoundObjectResult TicketNotFound(NotFoundException ex) =>
        NotFound(new ProblemDetails
        {
            Title = "Ticket not found",
            Detail = ex.Message,
            Status = StatusCodes.Status404NotFound
        });
}
