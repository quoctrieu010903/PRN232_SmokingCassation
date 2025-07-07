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
    public class UserPackageConfiguration : IEntityTypeConfiguration<UserPackage>
    {
        public void Configure(EntityTypeBuilder<UserPackage> builder)
        {



        }
    }
}
