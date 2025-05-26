using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmokingCessation.Domain.Entities;

namespace SmokingCessation.Infrastracture.Data.EntityConfigurations
{
    public class RankingConfiguration : IEntityTypeConfiguration<Ranking>
    {
        public void Configure(EntityTypeBuilder<Ranking> builder)
        {
            builder.HasOne(r => r.User)
              .WithOne(u => u.Ranking)
              .HasForeignKey<Ranking>(r => r.UserId);
        }
    }
}
