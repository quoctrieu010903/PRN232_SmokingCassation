
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmokingCessation.Domain.Entities;

namespace SmokingCessation.Infrastracture.Data.EntityConfigurations
{
    public class ProgressLogConfiguration : IEntityTypeConfiguration<ProgressLog>
    {
        public void Configure(EntityTypeBuilder<ProgressLog> builder)
        {
            builder.Property(p => p.Note).HasMaxLength(1000);
            builder.Property(p=>p.Status).HasConversion<int>();

            builder.HasOne(p => p.QuitPlan)
                   .WithMany(q => q.ProgressLogs)
                   .HasForeignKey(p => p.QuitPlanId);
        }
    }
}
