
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Identity;
using SmokingCessation.Core.Base;
using SmokingCessation.Domain.Enums;
namespace SmokingCessation.Domain.Entities
{
    public class ApplicationUser : IdentityUser<Guid>  
    {
        // Personal Info
        public string FullName { get; set; } = null!;

        // Audit Info
        public string? CreatedBy { get; set; }
        public string? LastUpdatedBy { get; set; }
        public string? DeletedBy { get; set; }

        public DateTimeOffset CreatedTime { get; set; }
        public DateTimeOffset LastUpdatedTime { get; set; }
        public DateTimeOffset? DeletedTime { get; set; }

        // Token Handling
        public string? ResetToken { get; set; }
        public DateTimeOffset? ResetTokenExpires { get; set; }
        public string? VerificationToken { get; set; }
        public DateTimeOffset? VerificationTokenExpires { get; set; }

        // Membership
        public MembershipType MembershipType { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation Properties
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public ICollection<QuitPlan> QuitPlans { get; set; } = new List<QuitPlan>();
        public ICollection<UserAchievement> UserAchievements { get; set; } = new List<UserAchievement>();
        public ICollection<Blog> Blogs { get; set; } = new List<Blog>();
        public Ranking? Ranking { get; set; }

    }
}
