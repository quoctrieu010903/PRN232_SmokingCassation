
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmokingCessation.Domain.Entities;

namespace SmokingCessation.Infrastracture.EntityConfigurations
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.HasIndex(u => u.Email).IsUnique();

            builder.Property(u => u.FullName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(u => u.MembershipType)
                   .HasConversion<int>(); 

            builder.HasMany(u => u.Payments)
                   .WithOne(p => p.User)
                   .HasForeignKey(p => p.UserId);

            builder.HasMany(u => u.QuitPlans)
                   .WithOne(q => q.User)
                   .HasForeignKey(q => q.UserId);

            builder.HasMany(u => u.UserAchievements)
                   .WithOne(ua => ua.User)
                   .HasForeignKey(ua => ua.UserId);

            builder.HasMany(u => u.Blogs)
                   .WithOne(b => b.Author)
                   .HasForeignKey(b => b.AuthorId);

            builder.HasOne(u => u.Ranking)
                   .WithOne(r => r.User)
                   .HasForeignKey<Ranking>(r => r.UserId);
        }
    }

}
