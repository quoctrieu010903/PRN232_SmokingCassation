
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
        }
    }

}
