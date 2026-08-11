using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RiseUp.Web.Models;

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

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        // POST: /Account/Register
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
                    // রোল ডাটাবেজে না থাকলে তৈরি হবে
                    if (!await _roleManager.RoleExistsAsync(model.UserRole))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(model.UserRole));
                    }

                    // ইউজারকে রোল প্রদান
                    await _userManager.AddToRoleAsync(user, model.UserRole);

                    // অ্যাকাউন্ট তৈরির পর স্বয়ংক্রিয় লগইন
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    // রেজিস্ট্রেশন শেষে রোল অনুযায়ী রিডাইরেক্ট
                    return RedirectToUserDashboard(model.UserRole);
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
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel());
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    // ReturnUrl থাকলে আগে সেখানে পাঠাবে
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) && returnUrl != "/")
                    {
                        return LocalRedirect(returnUrl);
                    }

                    // ইউজার এর রোল বের করে সংশ্লিষ্ট ড্যাশবোর্ডে রিডাইরেক্ট
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    if (user != null)
                    {
                        var roles = await _userManager.GetRolesAsync(user);
                        var primaryRole = roles.FirstOrDefault();

                        if (!string.IsNullOrEmpty(primaryRole))
                        {
                            return RedirectToUserDashboard(primaryRole);
                        }
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

        // Helper method: রোল অনুযায়ী ড্যাশবোর্ড রিডাইরেকশন হ্যান্ডলার
        private IActionResult RedirectToUserDashboard(string role)
        {
            return role switch
            {
                "Founder" => RedirectToAction("Dashboard", "Founder"),
                "Investor" => RedirectToAction("Dashboard", "Investor"),
                "Mentor" => RedirectToAction("Dashboard", "Mentor"),
                _ => RedirectToAction("Index", "Home")
            };
        }
    }
}