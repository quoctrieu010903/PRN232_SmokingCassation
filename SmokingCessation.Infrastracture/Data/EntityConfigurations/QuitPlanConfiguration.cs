
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmokingCessation.Domain.Entities;

namespace SmokingCessation.Infrastracture.Data.EntityConfigurations
{
    public class QuitPlanConfiguration : IEntityTypeConfiguration<QuitPlan>
    {
        public void Configure(EntityTypeBuilder<QuitPlan> builder)
        {
            builder.Property(q => q.Status).HasConversion<int>();
            builder.Property(q => q.Reason).HasMaxLength(1000);

            // Relationships
            builder.HasOne(q => q.User)
            .WithMany(u => u.QuitPlans)
            .HasForeignKey(q => q.UserId)
            .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(q => q.ProgressLogs)
                .WithOne(p => p.QuitPlan)
                .HasForeignKey(p => p.QuitPlanId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(q => q.AdviceLogs)
                .WithOne(a => a.QuitPlan)
                .HasForeignKey(a => a.QuitPlanId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
