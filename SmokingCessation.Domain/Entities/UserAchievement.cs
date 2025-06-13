using SmokingCessation.Core.Base;

namespace SmokingCessation.Domain.Entities
{
    public class UserAchievement : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid AchievementId { get; set; }
        public DateTime GrantedAt { get; set; }
        public ApplicationUser User { get; set; }
        public Achievement Achievement { get; set; }

    }
}