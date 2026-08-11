using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RiseUp.Web.Controllers
{
    [Authorize(Roles = "Mentor")]
    public class MentorController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}