using Microsoft.AspNetCore.Identity;

namespace Panda.Models
{
    // Utökar IdentityUser med egna fält
    public class AppUser : IdentityUser
    {
        public string? FullName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
