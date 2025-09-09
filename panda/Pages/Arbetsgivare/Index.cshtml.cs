using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using panda.Models;
using Panda.Data;
using Panda.Models;

namespace Panda.Pages.Arbetsgivare
{
    [Authorize(Roles = "Arbetsgivare")]
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

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            Assignments = await _db.Assignments
                .Include(a => a.Talent)
                .Include(a => a.Mentor)
                .Where(a => a.EmployerId == user.Id)
                .ToListAsync();
        }
    }
}
