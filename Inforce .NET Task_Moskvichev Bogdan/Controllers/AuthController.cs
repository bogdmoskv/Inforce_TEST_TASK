using Inforce_.NET_Task_Moskvichev_Bogdan.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Inforce_.NET_Task_Moskvichev_Bogdan.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public AuthController(IConfiguration configuration, ApplicationDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("login")]
        public IActionResult Login(LoginUserDTO userToLogin)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == userToLogin.Email);

            if (user == null || !user.VerifyPassword(userToLogin.Password))
            {
                return Unauthorized("Invalid email or password");
            }

            var claims = new[]
           {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),             
            };

            var issuer = _configuration["AuthOptions:Issuer"];
            var audience = _configuration["AuthOptions:Audience"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AuthOptions:Key"]));

            

            var jwt = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)), // Измените срок действия токена по вашему усмотрению
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            var token = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = token,
                username = userToLogin.Email
            };

            return Json(response);
        }
    }
}
