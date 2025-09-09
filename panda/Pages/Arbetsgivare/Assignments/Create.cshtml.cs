using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using panda.Models;
using Panda.Data;
using Panda.Models;

namespace Panda.Pages.Arbetsgivare.Assignments
{
    [Authorize(Roles = "Arbetsgivare")]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<AppUser> _userManager;

        public CreateModel(ApplicationDbContext db, UserManager<AppUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        [BindProperty]
        public string TalentId { get; set; }

        [BindProperty]
        public string MentorId { get; set; }

        public List<AppUser> Talents { get; set; } = new();
        public List<AppUser> Mentors { get; set; } = new();

        public async Task OnGetAsync()
        {
            Talents = (await _userManager.GetUsersInRoleAsync("Talang")).ToList();
            Mentors = (await _userManager.GetUsersInRoleAsync("Mentor")).ToList();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var employer = await _userManager.GetUserAsync(User);

            if (string.IsNullOrEmpty(TalentId) || string.IsNullOrEmpty(MentorId))
            {
                ModelState.AddModelError("", "Du måste välja både Talang och Mentor.");
                await OnGetAsync();
                return Page();
            }

            var assignment = new Assignment
            {
                EmployerId = employer.Id,
                TalentId = TalentId,
                MentorId = MentorId,
                StartDate = DateTime.UtcNow
            };

            _db.Assignments.Add(assignment);
            await _db.SaveChangesAsync();

            return RedirectToPage("/Arbetsgivare/Index");
        }
    }
}
