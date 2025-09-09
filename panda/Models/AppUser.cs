using Microsoft.AspNetCore.Identity;
using panda.Models;

namespace Panda.Models
{
    public class AppUser : IdentityUser
    {
        public string? FullName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // navigation properties
        public ICollection<Assignment> AssignmentsAsEmployer { get; set; } = new List<Assignment>();
        public ICollection<Assignment> AssignmentsAsTalent { get; set; } = new List<Assignment>();
        public ICollection<Assignment> AssignmentsAsMentor { get; set; } = new List<Assignment>();
    }
}
