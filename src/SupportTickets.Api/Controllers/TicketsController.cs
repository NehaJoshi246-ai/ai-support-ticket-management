using Microsoft.AspNetCore.Mvc;
using SupportTickets.Api.DTOs.Tickets;
using SupportTickets.Api.Services;
using SupportTickets.Domain.Exceptions;

namespace SupportTickets.Api.Controllers;

[ApiController]
[Route("api/tickets")]
[Produces("application/json")]
public class TicketsController : ControllerBase
{
    private readonly TicketService _tickets;
    private readonly TicketStatusTransitionService _transitions;

    public TicketsController(TicketService tickets, TicketStatusTransitionService transitions)
    {
        _tickets = tickets;
        _transitions = transitions;
    }

    /// <summary>List tickets, newest first.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<TicketResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<TicketResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var tickets = await _tickets.GetAllAsync(cancellationToken);
        return Ok(tickets);
    }

    /// <summary>Get a ticket by id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TicketResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TicketResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var ticket = await _tickets.GetByIdAsync(id, cancellationToken);
        if (ticket is null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Ticket not found",
                Detail = $"Ticket with id {id} was not found.",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(ticket);
    }

    /// <summary>Create a ticket (status starts as Open).</summary>
    [HttpPost]
    [ProducesResponseType(typeof(TicketResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TicketResponse>> Create(
        [FromBody] CreateTicketRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var created = await _tickets.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ValidationException ex)
        {
            return ValidationProblem(ex);
        }
    }

    /// <summary>Update title, description, priority, and assignee. Status is not allowed.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(TicketResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TicketResponse>> Update(
        int id,
        [FromBody] UpdateTicketRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var updated = await _tickets.UpdateAsync(id, request, cancellationToken);
            return Ok(updated);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Ticket not found",
                Detail = ex.Message,
                Status = StatusCodes.Status404NotFound
            });
        }
        catch (ValidationException ex)
        {
            return ValidationProblem(ex);
        }
    }

    /// <summary>
    /// Transition ticket status using TransitionMap.
    /// Invalid lifecycle moves return 409 Conflict (not 400).
    /// </summary>
    [HttpPatch("{id:int}/status")]
    [ProducesResponseType(typeof(TicketResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TicketResponse>> TransitionStatus(
        int id,
        [FromBody] TransitionStatusRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var updated = await _transitions.TransitionAsync(id, request, cancellationToken);
            return Ok(updated);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Ticket not found",
                Detail = ex.Message,
                Status = StatusCodes.Status404NotFound
            });
        }
        catch (InvalidTransitionException ex)
        {
            return Conflict(CreateInvalidTransitionProblem(ex));
        }
    }

    private static ProblemDetails CreateInvalidTransitionProblem(InvalidTransitionException ex)
    {
        var problem = new ProblemDetails
        {
            Title = "Invalid status transition",
            Detail = ex.Message,
            Status = StatusCodes.Status409Conflict,
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.10"
        };

        problem.Extensions["fromStatus"] = ex.FromStatus.ToString();
        problem.Extensions["toStatus"] = ex.ToStatus.ToString();
        problem.Extensions["allowedNextStatuses"] = ex.AllowedNextStatuses
            .Select(s => s.ToString())
            .ToArray();

        return problem;
    }

    private ActionResult ValidationProblem(ValidationException ex)
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
