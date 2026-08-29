using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RiseUp_Community.Data;
using RiseUp_Community.Models;
using System.Linq;
using System.Threading.Tasks;

namespace RiseUp_Community.Controllers
{
    [Authorize(Roles = "Startup,Founder")]
    public class FounderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public FounderController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Founder/Index
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            // Load Founder's Pitches
            var userPitches = await _context.Pitches
                .Where(p => p.UserId == user.Id)
                .ToListAsync();

            // Calculate Analytics
            var pitchIds = userPitches.Select(p => p.Id).ToList();
            int totalViews = await _context.PitchViews.CountAsync(pv => pitchIds.Contains(pv.PitchId));
            int totalInterests = await _context.Interests.CountAsync(i => pitchIds.Contains(i.PitchId));

            // Load Investors and Mentors for Browse Sections
            var investors = await _userManager.GetUsersInRoleAsync("Investor");
            var mentors = await _userManager.GetUsersInRoleAsync("Mentor");

            var viewModel = new StartupDashboardViewModel
            {
                Pitches = userPitches,
                TotalViews = totalViews,
                TotalInterests = totalInterests,
                AvailableInvestors = investors.ToList(),
                AvailableMentors = mentors.ToList()
            };

            return View(viewModel);
        }

        // POST: /Founder/CreatePitch
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePitch(Pitch pitch)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            if (ModelState.IsValid)
            {
                pitch.UserId = user.Id;
                pitch.CreatedAt = System.DateTime.UtcNow;

                _context.Pitches.Add(pitch);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Pitch created successfully!";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Failed to create pitch. Please check input values.";
            return RedirectToAction(nameof(Index));
        }
    }
}