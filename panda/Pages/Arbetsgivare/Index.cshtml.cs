using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Panda.Models;

namespace Panda.Pages.Arbetsgivare
{
    [Authorize(Roles = "Arbetsgivare")]
    public class IndexModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;

        public IndexModel(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public IList<AppUser> AllUsers { get; set; } = new List<AppUser>();

        public void OnGet()
        {
            AllUsers = _userManager.Users.ToList();
        }
    }
}
