using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmokingCessation.Domain.Entities;

namespace SmokingCessation.Infrastracture.Data
{
    public class SmokingCassationDBContext(DbContextOptions<SmokingCassationDBContext> options) : IdentityDbContext<User> (options)    {
        
    }
}
