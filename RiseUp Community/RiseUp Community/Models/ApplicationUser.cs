using Microsoft.AspNetCore.Identity;

namespace RiseUp_Community.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public string UserRole { get; set; } = string.Empty; // "Startup", "Investor", "Admin"
        public string? Role { get; set; } // <-- এই প্রপার্টিটি মিসিং ছিল
    }
}