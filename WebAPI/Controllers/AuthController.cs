using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public AuthController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        [HttpPost]
        public IActionResult Authenticate([FromBody] Credential credential)
        {
            //verifying the credential
            if(credential.UserName == "admin" && credential.Password == "password")
            {
                //creating the security context
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, "admin"),
                    new Claim(ClaimTypes.Email, "admin@admin.com"),
                    new Claim("Department", "HR"),
                    //new Claim("Admin",""),
                    new Claim("Manager", ""),
                    new Claim("EmploymentDate", "2021-02-01")
                };

                var expiresAt = DateTime.UtcNow.AddMinutes(10);
                return Ok(new
                {
                    access_token = CreateToken(claims, expiresAt),
                    expires_at = expiresAt
                });

            }
            ModelState.AddModelError("Unauthorized", "You are not authorized to access the endpoint.");
            return Unauthorized(ModelState);

        }
        private string CreateToken(IEnumerable<Claim> claims, DateTime expiresAt)
        {
            var secretKey = Encoding.ASCII.GetBytes(configuration.GetValue<string>("SecretKey"));
            var jwt = new JwtSecurityToken(claims: claims,
                                         notBefore: DateTime.UtcNow,
                                         expires: expiresAt,
                                         signingCredentials: new Microsoft.IdentityModel.Tokens.SigningCredentials(
                                             new SymmetricSecurityKey(secretKey),
                                             SecurityAlgorithms.HmacSha256Signature));

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
    public class Credential
    {
        public string UserName { get; set; }
        public string Password { get; set; }    
    }
}
