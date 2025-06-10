using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SmokingCessation.Domain.Entities;

namespace SmokingCessation.Infrastracture.Data.EntityConfigurations
{
            public class QuitPlanTaskConfiguration : IEntityTypeConfiguration<QuitPlanTask>
            {
                public void Configure(EntityTypeBuilder<QuitPlanTask> builder)
                {

                    builder.HasOne(t => t.PlanTemplate)
                      .WithMany(p => p.TaskTemplates)
                      .HasForeignKey(t => t.PlanTemplateId)
                      .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
