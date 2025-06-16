

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmokingCessation.Domain.Entities;

namespace SmokingCessation.Infrastracture.Data.EntityConfigurations
{
    public class UserMemberShipConfiguration : IEntityTypeConfiguration<UserPackage>
    {
      

        public void Configure(EntityTypeBuilder<UserPackage> builder)
        {
            builder.HasOne(x => x.User)
                   .WithMany(u => u.UserPackages)
                   .HasForeignKey(x => x.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Package)
                   .WithMany(p => p.UserPackages)
                   .HasForeignKey(x => x.PackageId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
