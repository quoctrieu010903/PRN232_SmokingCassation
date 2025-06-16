
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SmokingCessation.Domain.Entities;
using System.Reflection.Emit;

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


            builder.Property(mp => mp.DurationMonths);
          
            builder.Property(mp => mp.Features); // Features có thể là null hoặc chuỗi dài

            builder.HasMany(p => p.Payments)
             .WithOne(p => p.Package)
             .HasForeignKey(p => p.PackageId)
             .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(x => x.UserPackages)
                   .WithOne(up => up.Package)
                   .HasForeignKey(up => up.PackageId)
                   .OnDelete(DeleteBehavior.Cascade);


        }
    }

}
