using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RiseUp.Web.Controllers
{
    [Authorize(Roles = "Founder")]
    public class FounderController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}