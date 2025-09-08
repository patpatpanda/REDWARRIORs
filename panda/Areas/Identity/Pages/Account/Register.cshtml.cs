using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Panda.Models;

namespace Panda.Areas.Identity.Pages.Account
{
    [Authorize(Roles = "Admin")]
   
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ILogger<RegisterModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = default!;

        public string? ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; } = new List<AuthenticationScheme>();

        public class InputModel
        {
            [Required]
            [Display(Name = "Fullständigt namn")]
            public string FullName { get; set; } = default!;

            [Required]
            [EmailAddress]
            [Display(Name = "E-post")]
            public string Email { get; set; } = default!;

            [Required]
            [StringLength(100, MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Lösenord")]
            public string Password { get; set; } = default!;

            [DataType(DataType.Password)]
            [Display(Name = "Bekräfta lösenord")]
            [Compare("Password", ErrorMessage = "Lösenorden matchar inte.")]
            public string ConfirmPassword { get; set; } = default!;

            [Required]
            [Display(Name = "Välj roll")]
            public string SelectedRole { get; set; } = "Talang";
        }

        public async Task OnGetAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var user = new AppUser
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    FullName = Input.FullName
                };

                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Användaren skapade ett nytt konto.");

                    // Lägg användaren i vald roll
                    await _userManager.AddToRoleAsync(user, Input.SelectedRole);

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }
    }
}
