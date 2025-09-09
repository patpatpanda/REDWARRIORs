using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using panda.Models;
using Panda.Data;
using Panda.Models;

namespace Panda.Pages.Arbetsgivare.Assignments
{
    [Authorize(Roles = "Arbetsgivare")]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public DeleteModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public Assignment Assignment { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Assignment = await _db.Assignments
                .Include(a => a.Talent)
                .Include(a => a.Mentor)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (Assignment == null) return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var assignment = await _db.Assignments.FindAsync(id);
            if (assignment == null) return NotFound();

            _db.Assignments.Remove(assignment);
            await _db.SaveChangesAsync();

            return RedirectToPage("/Arbetsgivare/Index");
        }
    }
}
