
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SmokingCessation.Domain.Entities;

namespace SmokingCessation.Infrastracture.Data.EntityConfigurations
{
    public class MembershipPackageConfiguration : IEntityTypeConfiguration<MembershipPackage>
    {
        public void Configure(EntityTypeBuilder<MembershipPackage> builder)
        {
            builder.Property(mp => mp.Name).IsRequired();
            builder.Property(mp => mp.Price).HasColumnType("decimal(18,2)");
            builder.Property(mp => mp.Description).HasMaxLength(1000);
            builder.Property(p => p.Type).HasConversion<int>();

            builder.HasMany(mp => mp.QuitPlans)
                  .WithOne(qp => qp.MembershipPackage)
                  .HasForeignKey(qp => qp.PackageId) // Đảm bảo QuitPlan có FK PackageId
                  .IsRequired(false); // Có thể QuitPlan không có PackageId (ví dụ: kế hoạch miễn phí)
            builder.Property(mp => mp.DurationMonths)   
                  .IsRequired(); // Thời hạn gói là bắt buộc cho mọi loại gói

            builder.Property(mp => mp.Features); // Features có thể là null hoặc chuỗi dài

        }
    }

}
