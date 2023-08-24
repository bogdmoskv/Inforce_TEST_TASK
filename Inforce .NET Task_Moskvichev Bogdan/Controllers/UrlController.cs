using Inforce_.NET_Task_Moskvichev_Bogdan.Models;
using Inforce_.NET_Task_Moskvichev_Bogdan.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;


namespace Inforce_.NET_Task_Moskvichev_Bogdan.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UrlController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UrlController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpPost("shorturl")]
        public async Task<IActionResult> CheckUrl(UrlDto url)
        {
            string token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(); 
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
            if (jwtToken == null)
            {
                return Unauthorized(); 
            }

            var claims = jwtToken.Claims;


            var userEmailClaim = claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");
            if (userEmailClaim == null)
            {
                return Unauthorized(); 
            }

            var userEmail = userEmailClaim.Value;


            var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);
            if (user == null)
            {
                return Unauthorized();
            }


            if (!Uri.TryCreate(url.Url, UriKind.Absolute, out var inputUrl))
                return BadRequest("Invalid url has been provided!");

            var existingUrl = await _context.Urls.FirstOrDefaultAsync(u => u.Url == url.Url);
            if (existingUrl != null)
            {
                return BadRequest("This URL already exists in the database.");
            }


            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890@az";
            var randomStr = new string(Enumerable.Repeat(chars, 8)
                 .Select(x => x[random.Next(x.Length)]).ToArray());

            var sUrl = new UrlManagement()
            {
                Url = url.Url,
                ShortUrl = randomStr,
                OwnerId=user.Id,
            };
          
            _context.Urls.Add(sUrl);
            await _context.SaveChangesAsync();

            var httpContext = ControllerContext.HttpContext;
            var result = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/{sUrl.ShortUrl}";
            sUrl.ShortUrl = result;
           
            return Ok(sUrl);

          
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAllUrls()
        {
            try
            {
                var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
                var urls = await _context.Urls.ToListAsync();
                
                urls = urls.Select(url => new UrlManagement
                {
                    Id = url.Id,
                    Url = url.Url,
                    ShortUrl = $"{baseUrl}/{url.ShortUrl}"
                }).ToList();

                return Ok(urls);
            }
            catch
            {
                return BadRequest("Error after getting list of urls!");
            }
        }


        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUrl(int id)
        {
            try
            {
                var url = await _context.Urls.FindAsync(id);

                if (url == null)
                {
                    return NotFound($"URL with ID {id} not found");
                }

                _context.Urls.Remove(url);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch
            {
                return BadRequest($"Error deleting URL with ID {id}");
            }
        }



        [HttpGet("getInfoAboutOneUrl/{id}")]
        public async Task<IActionResult> GetInfoAboutOneUrl(int id)
        {
            try
            {
                var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";

                var url = await _context.Urls.FirstOrDefaultAsync(u => u.Id == id);

                if (url == null)
                {
                    return NotFound(); 
                }

                url.ShortUrl = $"{baseUrl}/{url.ShortUrl}";

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == url.OwnerId);

                if (user != null)
                {
                    var urlInfo = new
                    {
                        url.Id,
                        url.Url,
                        url.ShortUrl,
                        user.Email,
                        url.OwnerId,
                        url.CreatedTime
                    };

                    return Ok(urlInfo);
                }

                return Ok(url);
            }
            catch
            {
                return BadRequest("Error while getting information about the URL!");
            }
        }
    }
}
