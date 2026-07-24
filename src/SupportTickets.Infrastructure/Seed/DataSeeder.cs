using Microsoft.EntityFrameworkCore;
using SupportTickets.Domain.Entities;
using SupportTickets.Domain.Enums;

namespace SupportTickets.Infrastructure.Seed;

/// <summary>
/// Seeds users (with roles) and sample tickets for local assessment demos.
/// </summary>
public static class DataSeeder
{
    private static readonly DateTimeOffset SeedBase = new(2026, 7, 1, 9, 0, 0, TimeSpan.Zero);

    public static void Seed(ModelBuilder modelBuilder)
    {
        SeedUsers(modelBuilder);
        SeedTickets(modelBuilder);
        SeedComments(modelBuilder);
    }

    private static void SeedUsers(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, Name = "Sam Customer", Email = "sam.customer@example.com", Role = UserRole.Customer },
            new User { Id = 2, Name = "Riley Customer", Email = "riley.customer@example.com", Role = UserRole.Customer },
            new User { Id = 3, Name = "Casey Customer", Email = "casey.customer@example.com", Role = UserRole.Customer },
            new User { Id = 4, Name = "Alex Agent", Email = "alex.agent@example.com", Role = UserRole.Agent },
            new User { Id = 5, Name = "Morgan Agent", Email = "morgan.agent@example.com", Role = UserRole.Agent },
            new User { Id = 6, Name = "Taylor Agent", Email = "taylor.agent@example.com", Role = UserRole.Agent },
            new User { Id = 7, Name = "Jordan Lead", Email = "jordan.lead@example.com", Role = UserRole.Lead },
            new User { Id = 8, Name = "Quinn Lead", Email = "quinn.lead@example.com", Role = UserRole.Lead },
            new User { Id = 9, Name = "Avery Admin", Email = "avery.admin@example.com", Role = UserRole.Admin },
            new User { Id = 10, Name = "Blake Admin", Email = "blake.admin@example.com", Role = UserRole.Admin });
    }

    private static void SeedTickets(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ticket>().HasData(
            new Ticket
            {
                Id = 1,
                Title = "Cannot reset password",
                Description = "Password reset email never arrives after submitting the forgot-password form.",
                Priority = TicketPriority.High,
                Status = TicketStatus.Open,
                CreatedById = 1,
                AssignedToId = 4,
                CreatedAt = SeedBase
            },
            new Ticket
            {
                Id = 2,
                Title = "Dashboard charts blank on Safari",
                Description = "Support dashboard KPI charts render empty in Safari 17; Chrome is fine.",
                Priority = TicketPriority.Medium,
                Status = TicketStatus.InProgress,
                CreatedById = 2,
                AssignedToId = 5,
                CreatedAt = SeedBase.AddHours(2)
            },
            new Ticket
            {
                Id = 3,
                Title = "Invoice PDF download fails",
                Description = "Clicking Download Invoice returns HTTP 500 for account ACCT-8891.",
                Priority = TicketPriority.Critical,
                Status = TicketStatus.Resolved,
                CreatedById = 3,
                AssignedToId = 6,
                CreatedAt = SeedBase.AddDays(1)
            },
            new Ticket
            {
                Id = 4,
                Title = "Typo on billing FAQ page",
                Description = "FAQ item 3 says 'reciept' instead of 'receipt'. Low urgency.",
                Priority = TicketPriority.Low,
                Status = TicketStatus.Closed,
                CreatedById = 1,
                AssignedToId = 4,
                CreatedAt = SeedBase.AddDays(-3)
            },
            new Ticket
            {
                Id = 5,
                Title = "Duplicate notification emails",
                Description = "Customer receives two identical ticket-update emails for every status change.",
                Priority = TicketPriority.Medium,
                Status = TicketStatus.Cancelled,
                CreatedById = 2,
                AssignedToId = null,
                CreatedAt = SeedBase.AddDays(-1)
            });
    }

    private static void SeedComments(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TicketComment>().HasData(
            new TicketComment
            {
                Id = 1,
                TicketId = 2,
                Body = "Reproduced on Safari 17.4. Looking at the Chart.js canvas init path.",
                CreatedById = 5,
                CreatedAt = SeedBase.AddHours(3)
            },
            new TicketComment
            {
                Id = 2,
                TicketId = 3,
                Body = "Root cause was a null invoice template id. Fix deployed to staging.",
                CreatedById = 6,
                CreatedAt = SeedBase.AddDays(1).AddHours(4)
            });
    }
}
