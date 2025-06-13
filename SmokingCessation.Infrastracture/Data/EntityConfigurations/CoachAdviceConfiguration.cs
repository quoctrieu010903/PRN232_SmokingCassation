
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SmokingCessation.Domain.Entities;

namespace SmokingCessation.Infrastracture.Data.EntityConfigurations
{
    public class CoachAdviceConfiguration : IEntityTypeConfiguration<CoachAdviceLog>
    {
        public void Configure(EntityTypeBuilder<CoachAdviceLog> builder)
        {
            builder.Property(a => a.AdviceDate)
          .IsRequired();

            builder.Property(a => a.AdviceText)
                .IsRequired()
                .HasMaxLength(2000); // hoặc nhiều hơn tùy nội dung từ AI
        }
    }
    }
