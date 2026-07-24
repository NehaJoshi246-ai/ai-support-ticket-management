using Microsoft.EntityFrameworkCore;
using SupportTickets.Domain.Entities;
using SupportTickets.Infrastructure.Seed;

namespace SupportTickets.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<TicketComment> Comments => Set<TicketComment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        DataSeeder.Seed(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }
}
