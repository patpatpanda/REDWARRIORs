using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using panda.Models;
using Panda.Models; // <- håll detta konsekvent

namespace Panda.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<FeedbackEntry> FeedbackEntries { get; set; }
        public DbSet<TalentCheckIn> TalentCheckIns { get; set; }
        public DbSet<MentorCheckIn> MentorCheckIns { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Assignment -> AppUser (Employer/Talent/Mentor), Restrict för att undvika multiple cascade paths
            builder.Entity<Assignment>()
                .HasOne(a => a.Employer)
                .WithMany(u => u.AssignmentsAsEmployer)
                .HasForeignKey(a => a.EmployerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Assignment>()
                .HasOne(a => a.Talent)
                .WithMany(u => u.AssignmentsAsTalent)
                .HasForeignKey(a => a.TalentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Assignment>()
                .HasOne(a => a.Mentor)
                .WithMany(u => u.AssignmentsAsMentor)
                .HasForeignKey(a => a.MentorId)
                .OnDelete(DeleteBehavior.Restrict);

            // TalentCheckIn – unik per (Assignment, Talent, WeekStart)
            builder.Entity<TalentCheckIn>()
                .HasIndex(x => new { x.AssignmentId, x.TalentId, x.WeekStart })
                .IsUnique();

            builder.Entity<TalentCheckIn>()
                .HasOne(x => x.Assignment)
                .WithMany() // lägg ev. ICollection<TalentCheckIn> på Assignment om du vill
                .HasForeignKey(x => x.AssignmentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<TalentCheckIn>()
                .HasOne(x => x.Talent)
                .WithMany() // <-- Viktigt: INTE AssignmentsAsTalent (fel typ)
                .HasForeignKey(x => x.TalentId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.Entity<MentorCheckIn>()
        .HasIndex(x => new { x.AssignmentId, x.MentorId, x.WeekStart })
        .IsUnique();

            builder.Entity<MentorCheckIn>()
                .HasOne(x => x.Assignment)
                .WithMany() // lägg ev. ICollection<MentorCheckIn> på Assignment senare
                .HasForeignKey(x => x.AssignmentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<MentorCheckIn>()
                .HasOne(x => x.Mentor)
                .WithMany() // vill du kan du skapa u.Navigation t.ex. MentorCheckIns
                .HasForeignKey(x => x.MentorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
