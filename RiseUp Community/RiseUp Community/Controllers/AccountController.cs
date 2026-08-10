using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RiseUp.Web.Models;
using System.Threading.Tasks;

namespace RiseUp.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // রোল ডাটাবেজে না থাকলে তৈরি করা হবে
                    if (!await _roleManager.RoleExistsAsync(model.UserRole))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(model.UserRole));
                    }

                    // ইউজারকে নির্বাচিত রোল এসাইন করা
                    await _userManager.AddToRoleAsync(user, model.UserRole);

                    // অটোমেটিক লগইন করা
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }
    }
}