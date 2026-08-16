using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RiseUp.Web.Models;
using RiseUp_Community.Models;

namespace RiseUp_Community.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    Role = model.Role
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // 1. Identity System-এ Role Assign করা
                    if (!string.IsNullOrEmpty(model.Role))
                    {
                        await _userManager.AddToRoleAsync(user, model.Role);
                    }

                    // 2. Auto Sign-in
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    // 3. Role অনুযায়ী Redirect
                    return await RedirectToDashboardByUser(user);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    model.Email,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    if (user != null)
                    {
                        return await RedirectToDashboardByUser(user);
                    }
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            return View(model);
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // Helper Method: নিখুঁতভাবে Role চেক করে ড্যাশবোর্ডে পাঠানোর জন্য
        private async Task<IActionResult> RedirectToDashboardByUser(ApplicationUser user)
        {
            // Identity Roles থেকে চেক করা
            var roles = await _userManager.GetRolesAsync(user);
            string userRole = roles.FirstOrDefault() ?? user.Role ?? "";

            if (userRole.Equals("Investor", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Investor", "Dashboard");
            }
            else if (userRole.Equals("Startup", StringComparison.OrdinalIgnoreCase) ||
                     userRole.Equals("Founder", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Dashboard", "Founder");
            }
            else if (userRole.Equals("Mentor", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Index", "Mentor"); // অথবা Mentor এর নির্দিষ্ট Action
            }

            return RedirectToAction("Index", "Home");
        }
    }
}