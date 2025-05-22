
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Identity;
using SmokingCessation.Core.Base;
namespace SmokingCessation.Domain.Entities
{
    public class ApplicationUser : IdentityUser<Guid>  
    {
        public string FullName { get; set; }
        public string? CreatedBy { get; set; }
        public string? LastUpdatedBy { get; set; }
        public string? DeletedBy { get; set; }
        public DateTimeOffset CreatedTime { get; set; }
        public DateTimeOffset LastUpdatedTime { get; set; }
        public DateTimeOffset? DeletedTime { get; set; }
        public string? ResetToken { get; set; }
        public DateTimeOffset? ResetTokenExpires { get; set; }
        public string? VerificationToken { get; set; }
        public DateTimeOffset? VerificationTokenExpires { get; set; }

        public MembershipType MembershipType { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public ICollection<Payment> Payments { get; set; }
        public ICollection<QuitPlan> QuitPlans { get; set; }
        public ICollection<UserAchievement> UserAchievements { get; set; }
        public ICollection<Blog> Blogs { get; set; }
        public Ranking Ranking { get; set; }

    }
}
