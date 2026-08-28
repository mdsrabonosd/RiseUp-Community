using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace RiseUp_Community.Controllers
{
    public class InvestorController : Controller
    {
        // Sample Model structure for Startup Pitches
        public class PitchModel
        {
            public int Id { get; set; }
            public string StartupName { get; set; } = string.Empty;
            public string Category { get; set; } = string.Empty;
            public string Tagline { get; set; } = string.Empty;
            public decimal TargetGoal { get; set; }
            public double EquityOffered { get; set; }
            public bool IsSaved { get; set; }
        }

        public IActionResult Dashboard(string search = "", string category = "")
        {
            // Dummy data (বা আপনার DB Context থেকে ডাটা আনবেন)
            var pitches = new List<PitchModel>
            {
                new PitchModel { Id = 1, StartupName = "NextGen AI Analytics", Category = "FinTech", Tagline = "Automating B2B data analytics with AI.", TargetGoal = 200000, EquityOffered = 10 },
                new PitchModel { Id = 2, StartupName = "GreenPay Solutions", Category = "FinTech", Tagline = "Eco-friendly payment gateway for global trade.", TargetGoal = 150000, EquityOffered = 8 },
                new PitchModel { Id = 3, StartupName = "HealthPulse", Category = "HealthTech", Tagline = "AI-powered patient diagnostic assistance.", TargetGoal = 300000, EquityOffered = 12 },
                new PitchModel { Id = 4, StartupName = "EduSpark", Category = "EdTech", Tagline = "Interactive STEM learning platform for students.", TargetGoal = 100000, EquityOffered = 5 }
            };

            // Search filter
            if (!string.IsNullOrEmpty(search))
            {
                pitches = pitches.Where(p => p.StartupName.Contains(search, System.StringComparison.OrdinalIgnoreCase) ||
                                             p.Tagline.Contains(search, System.StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Category filter
            if (!string.IsNullOrEmpty(category))
            {
                pitches = pitches.Where(p => p.Category.Equals(category, System.StringComparison.OrdinalIgnoreCase)).ToList();
            }

            ViewBag.CurrentSearch = search;
            ViewBag.CurrentCategory = category;

            return View(pitches);
        }

        [HttpPost]
        public IActionResult RequestPitchDeck(int pitchId)
        {
            TempData["Success"] = "Pitch deck request sent successfully!";
            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        public IActionResult SaveStartup(int pitchId)
        {
            TempData["Success"] = "Startup saved to your bookmarks!";
            return RedirectToAction("Dashboard");
        }
    }
}