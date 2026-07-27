using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using SupportTickets.Api.DTOs.Tickets;
using SupportTickets.Domain.Enums;
using Xunit;

namespace SupportTickets.IntegrationTests;

[Collection(nameof(StatusTransitionMatrixCollection))]
public class TicketStatusTransitionMatrixTests
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly HttpClient _client;

    public TicketStatusTransitionMatrixTests(SupportTicketsWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Theory]
    [MemberData(nameof(StatusTransitionMatrixData.AllPairs), MemberType = typeof(StatusTransitionMatrixData))]
    public async Task PatchStatus_Matrix(TicketStatus from, TicketStatus to, bool shouldSucceed)
    {
        var ticketId = await CreateOpenTicketAsync();
        await PrepareTicketStatusAsync(ticketId, from);

        var response = await _client.PatchAsJsonAsync(
            $"/api/tickets/{ticketId}/status",
            new { status = to.ToString() });

        if (shouldSucceed)
        {
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var body = await response.Content.ReadFromJsonAsync<TicketResponse>(JsonOptions);
            Assert.NotNull(body);
            Assert.Equal(to, body.Status);
        }
        else
        {
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

            var problem = await response.Content.ReadFromJsonAsync<JsonElement>();
            Assert.Equal(JsonValueKind.Object, problem.ValueKind);
            Assert.True(problem.TryGetProperty("title", out var title));
            Assert.Equal("Invalid status transition", title.GetString());

            Assert.True(problem.TryGetProperty("fromStatus", out var fromEl));
            Assert.Equal(from.ToString(), fromEl.GetString());

            Assert.True(problem.TryGetProperty("toStatus", out var toEl));
            Assert.Equal(to.ToString(), toEl.GetString());

            Assert.True(problem.TryGetProperty("allowedNextStatuses", out var allowedEl));
            Assert.Equal(JsonValueKind.Array, allowedEl.ValueKind);
        }
    }

    private async Task<int> CreateOpenTicketAsync()
    {
        var response = await _client.PostAsJsonAsync("/api/tickets", new
        {
            title = $"Matrix test {Guid.NewGuid():N}",
            description = "Transition matrix integration test ticket.",
            priority = "Low",
            createdById = 1
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var ticket = await response.Content.ReadFromJsonAsync<TicketResponse>(JsonOptions);
        Assert.NotNull(ticket);
        Assert.Equal(TicketStatus.Open, ticket.Status);

        return ticket.Id;
    }

    private async Task PrepareTicketStatusAsync(int ticketId, TicketStatus target)
    {
        if (target == TicketStatus.Open)
        {
            return;
        }

        var steps = GetPreparationPath(target);
        foreach (var step in steps)
        {
            var response = await _client.PatchAsJsonAsync(
                $"/api/tickets/{ticketId}/status",
                new { status = step.ToString() });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        var ticket = await _client.GetFromJsonAsync<TicketResponse>($"/api/tickets/{ticketId}", JsonOptions);
        Assert.NotNull(ticket);
        Assert.Equal(target, ticket!.Status);
    }

    private static IReadOnlyList<TicketStatus> GetPreparationPath(TicketStatus target)
    {
        return target switch
        {
            TicketStatus.InProgress => new[] { TicketStatus.InProgress },
            TicketStatus.Resolved => new[] { TicketStatus.InProgress, TicketStatus.Resolved },
            TicketStatus.Closed => new[]
            {
                TicketStatus.InProgress,
                TicketStatus.Resolved,
                TicketStatus.Closed
            },
            TicketStatus.Cancelled => new[] { TicketStatus.Cancelled },
            _ => Array.Empty<TicketStatus>()
        };
    }
}
