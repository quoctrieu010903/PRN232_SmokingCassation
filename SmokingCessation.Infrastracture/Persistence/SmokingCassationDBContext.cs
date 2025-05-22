

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmokingCessation.Domain.Entities;
using System.Reflection;


namespace SmokingCessation.Infrastracture.Data
{
    public class SmokingCassationDBContext(DbContextOptions<SmokingCassationDBContext> options) : IdentityDbContext(options)
    {
      

        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<MembershipPackage> MembershipPackages { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<QuitPlan> QuitPlans { get; set; }
        public DbSet<ProgressLog> ProgressLogs { get; set; }
        public DbSet<Achievement> Achievements { get; set; }
        public DbSet<UserAchievement> UserAchievements { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Ranking> Rankings { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
     
    }
}
