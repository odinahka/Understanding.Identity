using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Web_App.Data.Account;
using Web_App.Services.Contracts;

namespace Web_App.Pages.Account
{
    public class LoginTwoFactorModel : PageModel
    {
        private readonly UserManager<User> userManager;
        private readonly IEmailService emailService;
        private readonly SignInManager<User> signInManager;

        [BindProperty]
        public EmailMFA EmailMFA { get; set; }

        public LoginTwoFactorModel(UserManager<User> userManager, IEmailService emailService, SignInManager<User> signInManager )
        {
            this.userManager = userManager;
            this.emailService = emailService;
            this.signInManager = signInManager;
            this.EmailMFA = new EmailMFA();
        }
        public async Task OnGetAsync(string email, bool rememberMe)
        {
            var user = await userManager.FindByEmailAsync(email);

            this.EmailMFA.RememberMe = rememberMe;
            this.EmailMFA.MFASecurityCode = String.Empty;
            // Generate the code
            var securityCode = await userManager.GenerateTwoFactorTokenAsync(user, "Email");

            // Send the code to the user
            await emailService.SendEmailAsync("odinaldo@mailtrap.io", email, "My App's OTP", $"Please use this code as the OTP: {securityCode}");
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
           var result =  await signInManager.TwoFactorSignInAsync("Email",EmailMFA.MFASecurityCode,EmailMFA.RememberMe, false);
            if (result.Succeeded) return RedirectToPage("/Index");
            else
            {
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("Login2FA", "You are locked out.");
                }
                else
                {
                    ModelState.AddModelError("Login2FA", "Failed to log in");
                }
                return Page();
            }
        }
    }
    public class EmailMFA
    {
        [Required]
        [Display(Name ="Security Code")]
        public string MFASecurityCode { get; set; }
        public bool RememberMe { get; set; }
    }
}
