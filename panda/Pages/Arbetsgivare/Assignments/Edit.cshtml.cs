using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using panda.Models;
using Panda.Data;
using Panda.Models;

namespace Panda.Pages.Arbetsgivare.Assignments
{
    [Authorize(Roles = "Arbetsgivare")]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<AppUser> _userManager;

        public EditModel(ApplicationDbContext db, UserManager<AppUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        [BindProperty]
        public Assignment Assignment { get; set; }

        public List<AppUser> Talents { get; set; } = new();
        public List<AppUser> Mentors { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Assignment = await _db.Assignments
                .Include(a => a.Talent)
                .Include(a => a.Mentor)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (Assignment == null) return NotFound();

            Talents = (await _userManager.GetUsersInRoleAsync("Talang")).ToList();
            Mentors = (await _userManager.GetUsersInRoleAsync("Mentor")).ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var assignment = await _db.Assignments.FindAsync(Assignment.Id);
            if (assignment == null) return NotFound();

            assignment.TalentId = Assignment.TalentId;
            assignment.MentorId = Assignment.MentorId;
            assignment.EndDate = Assignment.EndDate;

            await _db.SaveChangesAsync();
            return RedirectToPage("/Arbetsgivare/Index");
        }
    }
}
