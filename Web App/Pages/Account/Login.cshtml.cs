using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Web_App.Data.Account;

namespace Web_App.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<User> signInManager;

        public LoginModel(SignInManager<User> signInManager)
        {
            this.signInManager = signInManager;
        }
        [BindProperty]
        public CredentialVM Credential { get; set; }

        [BindProperty]
        public IEnumerable<AuthenticationScheme> ExternalLoginProviders { get; set; }
        public async Task OnGetAsync()
        {
            ExternalLoginProviders = await signInManager.GetExternalAuthenticationSchemesAsync();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
          var result =  await signInManager.PasswordSignInAsync(this.Credential.Email, this.Credential.Password, this.Credential.RememberMe, false);
            if (result.Succeeded) return RedirectToPage("/Index");
            else
            {
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("/Account/LoginTwoFactorWithAuthenticator", new {
                     RememberMe = this.Credential.RememberMe});
                }
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("Login", "You are locked out.");
                }
                else
                {
                    ModelState.AddModelError("Login", "Failed to log in");
                }
                return Page();
            }
        }
        public IActionResult OnPostLoginExternally(string provider)
        {
           var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, null);
            properties.RedirectUri = Url.Action("ExternalLoginCallBack", "Account");

            return Challenge(properties, provider);
        }
    }
    public class CredentialVM
    {
        [Required]
        [Display(Name = "Email")]
        public string Email{get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }

}
