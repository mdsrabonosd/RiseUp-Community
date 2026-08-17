using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RiseUp_Community.Models
{
    // ১. পিচ / স্টার্টআপ তথ্য
    public class Pitch
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal TargetAmount { get; set; }
        public decimal EquityOffered { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
        public string PitchDeckPath { get; set; } = string.Empty;
        public int ViewsCount { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string UserId { get; set; } = string.Empty;
        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }
    }

    // ২. ইনভেস্টরের আগ্রহ (Express Interest)
    public class Interest
    {
        public int Id { get; set; }
        public int PitchId { get; set; }
        [ForeignKey("PitchId")]
        public Pitch? Pitch { get; set; }

        public string InvestorId { get; set; } = string.Empty;
        [ForeignKey("InvestorId")]
        public ApplicationUser? Investor { get; set; }

        public string Status { get; set; } = "Interested"; // Interested, In Discussion, Closed
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    // ৩. বুকমার্ক / সেভ করা স্টার্টআপ
    public class SavedStartup
    {
        public int Id { get; set; }
        public int PitchId { get; set; }
        [ForeignKey("PitchId")]
        public Pitch? Pitch { get; set; }

        public string InvestorId { get; set; } = string.Empty;
        [ForeignKey("InvestorId")]
        public ApplicationUser? Investor { get; set; }
    }

    // ৪. ভিউ ট্র্যাকিং
    public class PitchView
    {
        public int Id { get; set; }
        public int PitchId { get; set; }
        public string InvestorId { get; set; } = string.Empty;
        public DateTime ViewedAt { get; set; } = DateTime.UtcNow;
    }

    // ৫. ফান্ডিং / বিনিয়োগের হিসাব
    public class Investment
    {
        public int Id { get; set; }
        public int PitchId { get; set; }
        [ForeignKey("PitchId")]
        public Pitch? Pitch { get; set; }

        public string InvestorId { get; set; } = string.Empty;
        [ForeignKey("InvestorId")]
        public ApplicationUser? Investor { get; set; }

        public decimal Amount { get; set; }
        public DateTime InvestedAt { get; set; } = DateTime.UtcNow;
    }
}