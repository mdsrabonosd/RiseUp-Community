using Microsoft.AspNetCore.Mvc;

namespace RiseUp_Community.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
