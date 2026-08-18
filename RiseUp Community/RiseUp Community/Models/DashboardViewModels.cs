using System;
using System.Collections.Generic;

namespace RiseUp_Community.Models
{
    // ১. Startup Dashboard ViewModel
    public class StartupDashboardViewModel
    {
        public int TotalPitches { get; set; }
        public int TotalViews { get; set; }
        public int InvestorInterests { get; set; }
        public decimal TotalFundingRaised { get; set; }
        public List<PitchSummaryDto> MyPitches { get; set; } = new();
        public List<InvestorInterestDto> RecentInterests { get; set; } = new();
    }

    public class PitchSummaryDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal TargetAmount { get; set; }
        public decimal EquityOffered { get; set; }
        public string Status { get; set; } = string.Empty;
        public int ViewsCount { get; set; }
    }

    public class InvestorInterestDto
    {
        public string InvestorName { get; set; } = string.Empty;
        public string PitchTitle { get; set; } = string.Empty;
        public DateTime DateExpressed { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    // ২. Investor Dashboard ViewModel
    public class InvestorDashboardViewModel
    {
        public int ReviewedPitchesCount { get; set; }
        public int SavedStartupsCount { get; set; }
        public int ActiveDiscussionsCount { get; set; }
        public decimal CommittedCapital { get; set; }
        public List<ExplorePitchDto> AvailablePitches { get; set; } = new();
    }

    public class ExplorePitchDto
    {
        public int Id { get; set; }
        public string StartupName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string ShortDescription { get; set; } = string.Empty;
        public decimal TargetAmount { get; set; }
        public decimal EquityOffered { get; set; }
        public string PitchDeckUrl { get; set; } = string.Empty;
        public bool IsSaved { get; set; }
        public bool HasExpressedInterest { get; set; }
    }

    // ৩. Admin Dashboard ViewModel
    public class AdminDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalStartups { get; set; }
        public int TotalInvestors { get; set; }
        public int PendingApprovals { get; set; }
        public List<PendingApprovalDto> PendingPitches { get; set; } = new();
    }

    public class PendingApprovalDto
    {
        public int PitchId { get; set; }
        public string PitchTitle { get; set; } = string.Empty;
        public string StartupOwner { get; set; } = string.Empty;
        public DateTime SubmittedDate { get; set; }
    }
}