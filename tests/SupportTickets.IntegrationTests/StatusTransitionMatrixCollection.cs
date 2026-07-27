using Xunit;

namespace SupportTickets.IntegrationTests;

[CollectionDefinition(nameof(StatusTransitionMatrixCollection), DisableParallelization = true)]
public sealed class StatusTransitionMatrixCollection : ICollectionFixture<SupportTicketsWebApplicationFactory>
{
}
