using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using Web_App.Data.Account;
using Web_App.Services.Contracts;

namespace Web_App.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<User> userManager;
        private readonly IEmailService emailService;

        public RegisterModel(UserManager<User> userManager, IEmailService emailService)
        {
            this.userManager = userManager;
            this.emailService = emailService;
        }
        [BindProperty]
        public RegisterVM RegisterVM { get; set; }
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if(!ModelState.IsValid) return Page();

            // Validating email address

            //Create the user
            var user = new User
            {
                Email = RegisterVM.Email,
                UserName = RegisterVM.Email

            };

          var result =  await this.userManager.CreateAsync(user, RegisterVM.Password);
            if (result.Succeeded)
            {
                await this.userManager.AddClaimsAsync(user, new List<Claim> { new Claim("Department", RegisterVM.Department), new Claim("Position", RegisterVM.Position) });
               var emailConfirmationToken = await this.userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = Url.PageLink(pageName: "/Account/ConfirmEmail",
                    values: new {userId = user.Id, token = emailConfirmationToken});
                await emailService.SendEmailAsync("odinaldo@mailtrap.io", user.Email,"Email Confirmation", $"Please click on this link to confirm your email address {confirmationLink}" );
               return RedirectToPage("/Account/Login");
            }
            else
            {
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("Regiser", error.Description);
                }
                return Page();
            }
        }
    }
    public class RegisterVM
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        public string Department { get; set; }
        [Required]
        public string Position { get; set; }
    }
}
