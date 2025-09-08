using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Panda.Models;

namespace Panda.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Här kan du lägga till egna tabeller
        // public DbSet<TalentProfile> TalentProfiles { get; set; }
        // public DbSet<MentorProfile> MentorProfiles { get; set; }
        // public DbSet<EmployerProfile> EmployerProfiles { get; set; }
    }
}
