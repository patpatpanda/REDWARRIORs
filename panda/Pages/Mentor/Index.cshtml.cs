using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using panda.Models;
using Panda.Data;
using Panda.Models;

namespace Panda.Pages.Mentor
{
    [Authorize(Roles = "Mentor")]
    public class IndexModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _db;

        public IndexModel(UserManager<AppUser> userManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        public IList<Assignment> Assignments { get; set; } = new List<Assignment>();

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            Assignments = await _db.Assignments
                .Include(a => a.Employer)
                .Include(a => a.Talent)
                .Where(a => a.MentorId == user.Id && a.EndDate == null)
                .ToListAsync();

            // Samma “snabbväg” som Talang: om bara ett uppdrag ? direkt till Mentor/CheckIn
            if (Assignments.Count == 1)
                return RedirectToPage("/Mentor/CheckIn", new { assignmentId = Assignments[0].Id });

            return Page();
        }
    }
}
