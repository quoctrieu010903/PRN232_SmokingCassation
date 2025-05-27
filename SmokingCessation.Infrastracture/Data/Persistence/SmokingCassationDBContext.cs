using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmokingCessation.Domain.Entities;

namespace SmokingCessation.Infrastracture.Data.Persistence
{
    public class SmokingCassationDBContext : IdentityDbContext<
        ApplicationUser,
        IdentityRole<Guid>,
        Guid,
        IdentityUserClaim<Guid>,
        IdentityUserRole<Guid>,
        IdentityUserLogin<Guid>,
        IdentityRoleClaim<Guid>,
        IdentityUserToken<Guid>>
    {
        public SmokingCassationDBContext(DbContextOptions<SmokingCassationDBContext> options)
            : base(options) { }

        public DbSet<MembershipPackage> MembershipPackages { get; set; }
        public DbSet<ApplicationUser> Users { get; set; }
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
