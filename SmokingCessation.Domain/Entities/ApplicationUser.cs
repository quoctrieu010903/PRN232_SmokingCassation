
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
        public virtual ICollection<Payment> Payments { get; set; }
        public virtual ICollection<QuitPlan> QuitPlans { get; set; } 
        public virtual ICollection<UserAchievement> UserAchievements { get; set; } 
        public virtual ICollection<Blog> Blogs { get; set; } 
        public virtual Ranking? Ranking { get; set; }

    }
}
