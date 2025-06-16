    
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmokingCessation.Domain.Entities;

namespace SmokingCessation.Infrastracture.Data.EntityConfigurations
{
    public class UserAchievementConfiguration : IEntityTypeConfiguration<UserAchievement>
    {
        public void Configure(EntityTypeBuilder<UserAchievement> builder)
        {
            builder.HasOne(ua => ua.User)
              .WithMany(u => u.UserAchievements)
              .HasForeignKey(ua => ua.UserId);

            builder.HasOne(ua => ua.Achievement)
                   .WithMany(a => a.UserAchievements)
                   .HasForeignKey(ua => ua.AchievementId);
           
        }
    }
}
