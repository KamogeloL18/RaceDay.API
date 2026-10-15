using Microsoft.EntityFrameworkCore;
using RaceDay.API.Models;

namespace RaceDay.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<Organiser> Organisers { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Enrolment> Enrolments { get; set; }
        public DbSet<Result> Results { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Unique email
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // One-to-One: User -> Participant
            modelBuilder.Entity<Participant>()
                .HasOne(p => p.User)
                .WithOne(u => u.Participant)
                .HasForeignKey<Participant>(p => p.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-One: User -> Organiser
            modelBuilder.Entity<Organiser>()
                .HasOne(o => o.User)
                .WithOne(u => u.Organiser)
                .HasForeignKey<Organiser>(o => o.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many: Organiser -> Events
            modelBuilder.Entity<Event>()
                .HasOne(e => e.Organiser)
                .WithMany(o => o.Events)
                .HasForeignKey(e => e.OrganiserID)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many: Event -> Categories
            modelBuilder.Entity<Category>()
                .HasOne(c => c.Event)
                .WithMany(e => e.Categories)
                .HasForeignKey(c => c.EventID)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many: Participant -> Enrolments
            modelBuilder.Entity<Enrolment>()
                .HasOne(en => en.Participant)
                .WithMany(p => p.Enrolments)
                .HasForeignKey(en => en.ParticipantID)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many: Event -> Enrolments
            modelBuilder.Entity<Enrolment>()
                .HasOne(en => en.Event)
                .WithMany(e => e.Enrolments)
                .HasForeignKey(en => en.EventID)
                .OnDelete(DeleteBehavior.NoAction);

            // One-to-Many: Category -> Enrolments
            modelBuilder.Entity<Enrolment>()
                .HasOne(en => en.Category)
                .WithMany(c => c.Enrolments)
                .HasForeignKey(en => en.CategoryID)
                .OnDelete(DeleteBehavior.NoAction);

            // One-to-One: Enrolment -> Result
            modelBuilder.Entity<Result>()
                .HasOne(r => r.Enrolment)
                .WithOne(en => en.Result)
                .HasForeignKey<Result>(r => r.EnrolmentID)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique constraint: Participant can only enrol once per event
            modelBuilder.Entity<Enrolment>()
                .HasIndex(en => new { en.ParticipantID, en.EventID })
                .IsUnique();
        }
    }
}