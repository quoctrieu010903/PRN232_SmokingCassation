
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

            builder.HasOne(q => q.User)
                   .WithMany(u => u.QuitPlans)
                   .HasForeignKey(q => q.UserId);
          
            builder.HasOne(qp => qp.MembershipPackage)
             .WithMany(mp => mp.QuitPlans) // Bạn cần thêm ICollection<QuitPlan> QuitPlans vào MembershipPackage
             .HasForeignKey(qp => qp.PackageId)
             .IsRequired(false); // Hoặc true nếu mọi QuitPlan phải thuộc một Package

        }
    }
}
