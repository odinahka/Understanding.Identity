using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRCoder;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using Web_App.Data.Account;

namespace Web_App.Pages.Account
{
    [Authorize]
    public class AuthenticatorWithMFASetupModel : PageModel
    {
        private readonly UserManager<User> userManager;

        [BindProperty]
        public SetupMFAVM SetupMFAVM  { get; set; }

        [BindProperty]
        public bool Succeed { get; set; }

        public AuthenticatorWithMFASetupModel(UserManager<User> userManager)
        {
            this.userManager = userManager;
            SetupMFAVM = new SetupMFAVM();
            Succeed = false;
        }
        public async Task  OnGetAsync()
        {
           
            var user = await userManager.GetUserAsync(base.User);
            await userManager.ResetAuthenticatorKeyAsync(user);
            var key = await userManager.GetAuthenticatorKeyAsync(user);
            
            this.SetupMFAVM.Key = key;
            this.SetupMFAVM.QRCodeBytes = GenerateQRCodeBytes("MyWebApp", key, user.Email);
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            var user = await userManager.GetUserAsync(base.User);
            if(await userManager.VerifyTwoFactorTokenAsync(user,
                                                    userManager.Options.Tokens.AuthenticatorTokenProvider,
                                                    SetupMFAVM.SecurityCode))
            {
                await userManager.SetTwoFactorEnabledAsync(user, true);
                Succeed = true;
            }
            else
            {
                ModelState.AddModelError("AuthenticatorSetup", "Something went wrong with authenticator setup, try again");
            }
            return Page();
        }
        private Byte[] GenerateQRCodeBytes(string provider, string key, string userEmail)
        {
            var qrCodeGenerator = new QRCodeGenerator();
            var qrCodeData = qrCodeGenerator.CreateQrCode($"otpauth://totp/{provider}?secret={key}&issuer={provider}", QRCodeGenerator.ECCLevel.Q);
            var qrCode = new QRCode(qrCodeData);
            var qrCodeImage = qrCode.GetGraphic(20);
            return BitmapToByteArray(qrCodeImage);

        }
        private Byte[] BitmapToByteArray(Bitmap image)
        {
            using(MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
            
        }
    }
    public class SetupMFAVM
    {
       
        public string Key { get; set; }
        [Required]
        public string SecurityCode { get; set; }
        public Byte[] QRCodeBytes { get; set; }
    }
}
