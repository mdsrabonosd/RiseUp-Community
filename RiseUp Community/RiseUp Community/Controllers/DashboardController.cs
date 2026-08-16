using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RiseUp_Community.Data;
using RiseUp_Community.Models;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RiseUp_Community.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // 1. STARTUP DASHBOARD
        // ==========================================
        [Authorize(Roles = "Startup")]
        public async Task<IActionResult> Startup()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var model = new StartupDashboardViewModel
            {
                TotalPitches = await _context.Pitches.CountAsync(p => p.UserId == userId),
                TotalViews = await _context.Pitches.Where(p => p.UserId == userId).SumAsync(p => p.ViewsCount),
                InvestorInterests = await _context.Interests.CountAsync(i => i.Pitch != null && i.Pitch.UserId == userId),
                TotalFundingRaised = await _context.Investments.Where(i => i.Pitch != null && i.Pitch.UserId == userId).SumAsync(i => i.Amount),

                MyPitches = await _context.Pitches
                    .Where(p => p.UserId == userId)
                    .Select(p => new PitchSummaryDto
                    {
                        Id = p.Id,
                        Title = p.Title,
                        Category = p.Category,
                        TargetAmount = p.TargetAmount,
                        EquityOffered = p.EquityOffered,
                        Status = p.Status,
                        ViewsCount = p.ViewsCount
                    }).ToListAsync(),

                RecentInterests = await _context.Interests
                    .Where(i => i.Pitch != null && i.Pitch.UserId == userId)
                    .OrderByDescending(i => i.CreatedAt)
                    .Take(5)
                    .Select(i => new InvestorInterestDto
                    {
                        InvestorName = i.Investor != null ? i.Investor.FullName : "Anonymous",
                        PitchTitle = i.Pitch != null ? i.Pitch.Title : "N/A",
                        DateExpressed = i.CreatedAt,
                        Status = i.Status
                    }).ToListAsync()
            };

            return View(model);
        }

        // ==========================================
        // 2. INVESTOR DASHBOARD
        // ==========================================
        [Authorize(Roles = "Investor")]
        public async Task<IActionResult> Investor(string search, string category)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var query = _context.Pitches.Where(p => p.Status == "Approved").AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Title.Contains(search) || p.Description.Contains(search));
            }

            if (!string.IsNullOrEmpty(category) && category != "All Categories")
            {
                query = query.Where(p => p.Category == category);
            }

            var pitchesList = await query.ToListAsync();

            var savedPitchIds = await _context.SavedStartups
                .Where(s => s.InvestorId == userId)
                .Select(s => s.PitchId)
                .ToListAsync();

            var interestedPitchIds = await _context.Interests
                .Where(i => i.InvestorId == userId)
                .Select(i => i.PitchId)
                .ToListAsync();

            var model = new InvestorDashboardViewModel
            {
                ReviewedPitchesCount = await _context.PitchViews.CountAsync(v => v.InvestorId == userId),
                SavedStartupsCount = savedPitchIds.Count,
                ActiveDiscussionsCount = await _context.Interests.CountAsync(i => i.InvestorId == userId && i.Status == "In Discussion"),
                CommittedCapital = await _context.Investments.Where(i => i.InvestorId == userId).SumAsync(i => i.Amount),

                AvailablePitches = pitchesList.Select(p => new ExplorePitchDto
                {
                    Id = p.Id,
                    StartupName = p.Title,
                    Category = p.Category,
                    ShortDescription = p.Description,
                    TargetAmount = p.TargetAmount,
                    EquityOffered = p.EquityOffered,
                    PitchDeckUrl = p.PitchDeckPath,
                    IsSaved = savedPitchIds.Contains(p.Id),
                    HasExpressedInterest = interestedPitchIds.Contains(p.Id)
                }).ToList()
            };

            return View(model);
        }

        // Action for Expressing Interest
        [HttpPost]
        [Authorize(Roles = "Investor")]
        public async Task<IActionResult> ExpressInterest(int pitchId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            bool alreadyExpressed = await _context.Interests.AnyAsync(i => i.PitchId == pitchId && i.InvestorId == userId);

            if (!alreadyExpressed)
            {
                var newInterest = new Interest
                {
                    PitchId = pitchId,
                    InvestorId = userId,
                    Status = "Interested",
                    CreatedAt = DateTime.UtcNow
                };

                _context.Interests.Add(newInterest);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Investor));
        }

        // ==========================================
        // 3. ADMIN DASHBOARD
        // ==========================================
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Admin()
        {
            var model = new AdminDashboardViewModel
            {
                TotalUsers = await _context.Users.CountAsync(),
                TotalStartups = await _context.Users.CountAsync(u => u.UserRole == "Startup"),
                TotalInvestors = await _context.Users.CountAsync(u => u.UserRole == "Investor"),
                PendingApprovals = await _context.Pitches.CountAsync(p => p.Status == "Pending"),

                PendingPitches = await _context.Pitches
                    .Where(p => p.Status == "Pending")
                    .Select(p => new PendingApprovalDto
                    {
                        PitchId = p.Id,
                        PitchTitle = p.Title,
                        StartupOwner = p.User != null ? p.User.FullName : "N/A",
                        SubmittedDate = p.CreatedAt
                    }).ToListAsync()
            };

            return View(model);
        }

        // Action for Admin Approval
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApprovePitch(int pitchId)
        {
            var pitch = await _context.Pitches.FindAsync(pitchId);
            if (pitch != null)
            {
                pitch.Status = "Approved";
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Admin));
        }
    }
}