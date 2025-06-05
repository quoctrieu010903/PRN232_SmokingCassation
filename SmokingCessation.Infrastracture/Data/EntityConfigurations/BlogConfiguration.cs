
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SmokingCessation.Domain.Entities;

namespace SmokingCessation.Infrastracture.Data.EntityConfigurations
{
    public class BlogConfiguration : IEntityTypeConfiguration<Blog>
    {
        public void Configure(EntityTypeBuilder<Blog> builder)
        {
            builder.Property(b => b.Status).HasConversion<int>();

            builder.HasOne(b => b.Author)
                   .WithMany(u => u.Blogs)
                   .HasForeignKey(b => b.AuthorId);
            builder.HasMany(b=> b.Feedbacks)
                    .WithOne(f=>f.Blog)
                    .HasForeignKey(f => f.BlogId)
                    .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x=>x.Ratings)
                    .WithOne(f=>f.Blog)
                    .HasForeignKey(f=>f.BlogId)
                    .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
