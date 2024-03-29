using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Web_App.Data.Account;

namespace Web_App.Pages.Account
{
    public class LoginTwoFactorWithAuthenticatorModel : PageModel
    {
        private readonly SignInManager<User> signInManager;

        [BindProperty]
        public AuthenticatorMFA AuthenticatorMFA { get; set; }
        public LoginTwoFactorWithAuthenticatorModel(SignInManager<User> signInManager)
        {
            AuthenticatorMFA = new AuthenticatorMFA();
            this.signInManager = signInManager;
        }
        public void OnGet(bool rememberMe)
        {
            AuthenticatorMFA.SecurityCode = string.Empty;
            AuthenticatorMFA.RememberMe = rememberMe;
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

           var result = await signInManager.TwoFactorAuthenticatorSignInAsync(AuthenticatorMFA.SecurityCode, AuthenticatorMFA.RememberMe, false);
            if (result.Succeeded) return RedirectToPage("/Index");
            else
            {
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("Authenticator2FA", "You are locked out.");
                }
                else
                {
                    ModelState.AddModelError("Authenticator2FA", "Failed to log in");
                }
                return Page();
            }

        }
    }
}
public class AuthenticatorMFA
{
    [Required]
    [Display(Name = "Code")]
    public string SecurityCode { get; set; }
    public bool RememberMe { get; set; }
}
