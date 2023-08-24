using Inforce_.NET_Task_Moskvichev_Bogdan.Models;
using Inforce_.NET_Task_Moskvichev_Bogdan.Models.Authentication;
using Inforce_.NET_Task_Moskvichev_Bogdan.Models.DTO;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult Register(RegisterUserDTO model)
        {          
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("Email", "User with the provided email is already registered.");
                return BadRequest(ModelState);
            }

            var existingUser = _context.Users.FirstOrDefault(u => u.Email == model.Email);
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

            //var userRole = _context.Roles.FirstOrDefault(r => r.Name == "User");
            //if (userRole != null)
            //{
            //    user.UserRoles = new List<UserRole>
            //    {
            //        new UserRole { RoleId = userRole.Id }
            //    };
            //}


            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok("User registered successfully");
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
