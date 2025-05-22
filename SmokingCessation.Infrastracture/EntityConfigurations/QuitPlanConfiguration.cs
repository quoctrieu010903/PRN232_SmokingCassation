
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmokingCessation.Domain.Entities;

namespace SmokingCessation.Infrastracture.EntityConfigurations
{
    public class QuitPlanConfiguration : IEntityTypeConfiguration<QuitPlan>
    {
        public void Configure(EntityTypeBuilder<QuitPlan> builder)
        {
            builder.Property(q => q.Status).HasConversion<int>();
            builder.Property(q => q.Reason).HasMaxLength(1000);

            builder.HasOne(q => q.User)
                   .WithMany(u => u.QuitPlans)
                   .HasForeignKey(q => q.UserId);
        }
    }
}
