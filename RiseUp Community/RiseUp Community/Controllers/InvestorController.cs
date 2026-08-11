using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RiseUp.Web.Controllers
{
    [Authorize(Roles = "Investor")]
    public class InvestorController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}