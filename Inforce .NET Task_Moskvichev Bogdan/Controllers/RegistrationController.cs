using Inforce_.NET_Task_Moskvichev_Bogdan.Models;
using Inforce_.NET_Task_Moskvichev_Bogdan.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inforce_.NET_Task_Moskvichev_Bogdan.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RegistrationController : Controller
    {
        private readonly ApplicationDbContext _context;
        public RegistrationController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(RegisterUserDTO model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("Email", "User with the provided email is already registered.");
                return BadRequest(ModelState);
            }

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "User with the provided email is already registered.");
                return BadRequest(ModelState);
            }

            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Password and confirm password do not match");
                return BadRequest(ModelState);
            }

            var user = new User
            {
                Email = model.Email,
            };

            user.SetPassword(model.Password);
            user.Role = "User";

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("User registered successfully");
        }

    }
}
