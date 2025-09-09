using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using panda.Models;
using Panda.Data;
using Panda.Models;

namespace Panda.Pages.Talang
{
    [Authorize(Roles = "Talang")]
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
                .Include(a => a.Mentor)
                .Where(a => a.TalentId == user.Id && a.EndDate == null)
                .ToListAsync();

            if (Assignments.Count == 1)
                return RedirectToPage("/Talang/CheckIn", new { assignmentId = Assignments[0].Id });

            return Page();
        }

    }
}
