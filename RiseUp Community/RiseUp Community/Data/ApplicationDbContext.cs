using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RiseUp_Community.Models;

namespace RiseUp_Community.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Pitch> Pitches { get; set; }
        public DbSet<Interest> Interests { get; set; }
        public DbSet<SavedStartup> SavedStartups { get; set; }
        public DbSet<PitchView> PitchViews { get; set; }
        public DbSet<Investment> Investments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Cascade Delete সমস্যা সমাধানের জন্য Foreign Key Behavior কনফিগারেশন

            // 1. Interest Relationship
            builder.Entity<Interest>()
                .HasOne(i => i.Pitch)
                .WithMany()
                .HasForeignKey(i => i.PitchId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Interest>()
                .HasOne(i => i.Investor)
                .WithMany()
                .HasForeignKey(i => i.InvestorId)
                .OnDelete(DeleteBehavior.Restrict);

            // 2. SavedStartup Relationship
            builder.Entity<SavedStartup>()
                .HasOne(s => s.Pitch)
                .WithMany()
                .HasForeignKey(s => s.PitchId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<SavedStartup>()
                .HasOne(s => s.Investor)
                .WithMany()
                .HasForeignKey(s => s.InvestorId)
                .OnDelete(DeleteBehavior.Restrict);

            // 3. Investment Relationship
            builder.Entity<Investment>()
                .HasOne(inv => inv.Pitch)
                .WithMany()
                .HasForeignKey(inv => inv.PitchId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Investment>()
                .HasOne(inv => inv.Investor)
                .WithMany()
                .HasForeignKey(inv => inv.InvestorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}