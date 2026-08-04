using Microsoft.AspNetCore.Mvc;
using RiseUp.Web.Models;

namespace RiseUp.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View(new LandingViewModel());
        }
    }
}