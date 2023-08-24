using Inforce_.NET_Task_Moskvichev_Bogdan.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Inforce_.NET_Task_Moskvichev_Bogdan.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}