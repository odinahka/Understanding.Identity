using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Web_App.Data.Account;

namespace Web_App.Pages.Account
{
    [Authorize]
    public class UserProfileModel : PageModel
    {
        private readonly UserManager<User> userManager;
        [BindProperty]
        public UserProfileVM UserProfile { get; set; }
        [BindProperty]
        public string? SuccessMessage { get; set; }

        public UserProfileModel(UserManager<User> userManager)
        {
            this.userManager = userManager;
            this.UserProfile = new UserProfileVM();
            this.SuccessMessage = string.Empty;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            var (user, departmentClaim, positionClaim) = await GetUserInfoAsync();
            this.UserProfile.Department = departmentClaim?.Value;
            this.UserProfile.Position = positionClaim?.Value;
            this.UserProfile.Email = user.Email;
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {

            if (!ModelState.IsValid) return Page();
            try
            {
                var (user, departmentClaim, positionClaim) = await GetUserInfoAsync();
                await userManager.ReplaceClaimAsync(user, departmentClaim, new Claim(departmentClaim.Type, UserProfile.Department));
                await userManager.ReplaceClaimAsync(user, positionClaim, new Claim(positionClaim.Type, UserProfile.Position));
                this.SuccessMessage = "The user profile is saved successfully";

            }
            catch
            {
                ModelState.AddModelError("UserProfile", "Error occured when saving user profile");
            }
            return Page();

         }
        private async Task<(Data.Account.User, Claim, Claim)> GetUserInfoAsync()
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var claims = await userManager.GetClaimsAsync(user);
            var departmentClaim = claims.FirstOrDefault(x => x.Type == "Department");
            var positionClaim = claims.FirstOrDefault(x => x.Type == "Position");
            return (user, departmentClaim, positionClaim);
        }
    }
    public class UserProfileVM
    {
        public string Email { get; set; }
        [Required]
        public string? Department { get; set; }
        [Required]
        public string? Position { get; set; }
    }
}
