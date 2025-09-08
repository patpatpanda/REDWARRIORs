using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Panda.Models;

namespace Panda.Pages.Talang
{
    [Authorize(Roles = "Talang")]
    public class IndexModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;

        public IndexModel(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public AppUser CurrentUser { get; set; }

        public async Task OnGetAsync()
        {
            CurrentUser = await _userManager.GetUserAsync(User);
        }
    }
}
