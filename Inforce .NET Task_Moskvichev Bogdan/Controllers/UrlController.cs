using Inforce_.NET_Task_Moskvichev_Bogdan.Models;
using Inforce_.NET_Task_Moskvichev_Bogdan.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
                ShortUrl = randomStr
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

                return Ok($"Record with ID {id} was deleted!");
            }
            catch
            {
                return BadRequest($"Error deleting URL with ID {id}");
            }
        }


    }
}
