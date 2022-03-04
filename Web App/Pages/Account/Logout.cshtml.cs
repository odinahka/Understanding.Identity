using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web_App.Data.Account;

namespace Web_App.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly Microsoft.AspNetCore.Identity.SignInManager<User> signInManager;

        public LogoutModel(SignInManager<User> signInManager)
        {
            this.signInManager = signInManager;
        }
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAsync()
        {
           await signInManager.SignOutAsync();
            return RedirectToPage("/Account/Login");
        }
    }
}
