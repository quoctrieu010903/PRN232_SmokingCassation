using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmokingCessation.Domain.Entities;

namespace SmokingCessation.Infrastracture.Data.EntityConfigurations
{
    public class FeedBackConfiguration : IEntityTypeConfiguration<Feedback>
    {
        public void Configure(EntityTypeBuilder<Feedback> builder)
        {
        
            builder.Property(f => f.Comment)
                .IsRequired()
                .HasMaxLength(1000);
            builder.Property(f => f.IsApproved)
                .HasDefaultValue(false);

            // Relationships
            builder.HasOne(f => f.Blog)
                .WithMany(b => b.Feedbacks)
                .HasForeignKey(f => f.BlogId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            
            builder.HasIndex(f => f.BlogId);
            builder.HasIndex(f => f.UserId);


        }
    }
}
