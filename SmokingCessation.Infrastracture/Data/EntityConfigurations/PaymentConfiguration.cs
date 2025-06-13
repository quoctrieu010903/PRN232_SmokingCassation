
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmokingCessation.Domain.Entities;

namespace SmokingCessation.Infrastracture.Data.EntityConfigurations
{
    internal class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.Property(p => p.Amount).HasColumnType("decimal(18,2)");
            builder.Property(p => p.Status).HasConversion<int>();
           
            builder.HasOne(p => p.User)
           .WithMany(u => u.Payments)
           .HasForeignKey(p => p.UserId)
           .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Package)
                .WithMany(mp => mp.Payments)
                .HasForeignKey(p => p.PackageId)
                .OnDelete(DeleteBehavior.Restrict);



        }
    }

}
