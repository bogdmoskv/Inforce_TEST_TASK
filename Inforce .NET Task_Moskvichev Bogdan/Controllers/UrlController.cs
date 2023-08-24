using Inforce_.NET_Task_Moskvichev_Bogdan.Models;
using Inforce_.NET_Task_Moskvichev_Bogdan.Models.DTO;
using Microsoft.AspNetCore.Mvc;

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
        public async Task <IActionResult> CheckUrl(UrlDto url)
        {
            if (!Uri.TryCreate(url.Url, UriKind.Absolute, out var inputUrl))
                return BadRequest("Invalid url has been provided!");

            //Створюємо коротку версію URL
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890@az";
            var randomStr = new string(Enumerable.Repeat(chars, 8)
                 .Select(x => x[random.Next(x.Length)]).ToArray());

            var sUrl = new UrlManagement()
            {
                Url=url.Url,
                ShortUrl = randomStr
            };

            _context.Urls.Add(sUrl);
            await _context.SaveChangesAsync();

            var httpContext = ControllerContext.HttpContext;
            var result = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/{sUrl.ShortUrl}";
            return Ok(new UrlShortResponseDto()
            {
                Url = result
            });
        }


        //[HttpGet("{shortUrl}")]
        //public IActionResult HandleFallbackRequests()
        //{
        //    var httpContext = ControllerContext.HttpContext;
        //    var path = httpContext.Request.Path.ToUriComponent().Trim('/');
        //    var urlMatch = _context.Urls.FirstOrDefault(x => 
        //        x.ShortUrl.ToLower().Trim() == path.Trim());

        //    if (urlMatch == null)
        //        return BadRequest("Invalid request!");

        //    return Redirect(urlMatch.Url);
        //}

    }
}
