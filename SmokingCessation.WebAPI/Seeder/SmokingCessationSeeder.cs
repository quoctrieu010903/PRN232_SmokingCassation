    
using Microsoft.AspNetCore.Identity;
using SmokingCessation.Core.Constants;
using SmokingCessation.Core.Utils;
using SmokingCessation.Domain.Entities;
using SmokingCessation.Domain.Interfaces;
using SmokingCessation.Infrastracture.Data.Persistence;
using static System.Net.Mime.MediaTypeNames;

namespace SmokingCessation.Infrastracture.Seeder
{
    public class SmokingCessationSeeder(SmokingCassationDBContext _dbContext) : ISmokingSessationSeeder
    {
        public async Task Seed()
        {
            if (await _dbContext.Database.CanConnectAsync())
            {
                if (!_dbContext.Users.Any())
                {
                    _dbContext.Users.AddRange(GetUsers());
                }

                if (!_dbContext.Achievements.Any())
                {
                    _dbContext.Achievements.AddRange(GetAchievements());
                }

                if (!_dbContext.UserAchievements.Any())
                {
                    _dbContext.UserAchievements.AddRange(GetUserAchievements());
                }

                if (!_dbContext.Blogs.Any())
                {
                    _dbContext.Blogs.AddRange(GetBlogs());
                }
                if (!_dbContext.Roles.Any())
                {
                    var roles = GetRoles(); 
                    _dbContext.Roles.AddRange(roles);
                  
                }
                await _dbContext.SaveChangesAsync();


            }
        }

        public IEnumerable<IdentityRole<Guid>> GetRoles()
        {
            List<IdentityRole<Guid>> roles =
            [
                new (UserRoles.Member)
                {
                    NormalizedName = UserRoles.Member.ToUpper()
                },
                new (UserRoles.Coach)
                {
                    NormalizedName = UserRoles.Coach.ToUpper()
                },
                new (UserRoles.Admin)
                {
                       NormalizedName = UserRoles.Admin.ToUpper()
                },
            ];

            return roles;
        }

        public static List<ApplicationUser> GetUsers()
        {
            return new List<ApplicationUser>
            {

                new ApplicationUser
                {
                    Id = Guid.Parse("c1c780f9-8dce-41b3-9735-b6bc0e935712"),
                    UserName = "john.doe",
                    FullName = "John Doe",
                    Email = "john.doe@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Ab@123456"),
                    EmailConfirmed = true,
                    CreatedTime =  DateTime.UtcNow,
                     SecurityStamp = Guid.NewGuid().ToString()
                },
                new ApplicationUser
                {
                    Id = Guid.Parse("f77b8d8a-345e-4a63-8928-2ddbdcf7b93b"),
                    UserName = "jane.smith",
                    FullName = "Jane Smith",
                    Email = "jane.smith@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Ab@123456"),
                    EmailConfirmed = true,
                    CreatedTime =  DateTime.UtcNow,
                     SecurityStamp = Guid.NewGuid().ToString()
                },
                new ApplicationUser
                {
                    Id = Guid.Parse("75075487-7FA2-4063-9CE7-3C85D2D88A12"),
                    UserName = "quoctrieu15",
                    FullName = "Quoc Trieu",
                    Email = "luongquoctrieu165@gmail.com", // Sửa lỗi định dạng email
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Ab@123456"),
                    EmailConfirmed = true,
                    CreatedTime =  DateTime.UtcNow,
                     SecurityStamp = Guid.NewGuid().ToString()
                },
                new ApplicationUser
                {
                    Id = Guid.Parse("BC790193-647D-420F-BC27-C8ABB27BCB53"),
                    UserName = "ThanhVu22",
                    FullName = "Lê Thanh Vũ",
                    Email = "vult2911@gmail.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Ab@123456"),
                    EmailConfirmed = true,
                    CreatedTime =  DateTime.UtcNow,
                     SecurityStamp = Guid.NewGuid().ToString()
                },
                new ApplicationUser
                {
                    Id = Guid.Parse("A0CBD7C7-D6A3-4912-B8C7-3029B55B5E3D"),
                    UserName = "VietQuoc01",
                    FullName = "Nguyễn Trần Việt Quốc",
                    Email = "ntvq88@gmail.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Ab@123456"),
                    EmailConfirmed = true,
                    CreatedTime =  DateTime.UtcNow,
                    SecurityStamp = Guid.NewGuid().ToString()
                },
                new ApplicationUser
                {
                    Id = Guid.Parse("4BD6B6BC-58E9-4A2A-B1CE-3FF3F11D5D4C"),
                    UserName = "NhatTruong02",
                    FullName = "Lê Nhật Trường",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Ab@123456"),
                    Email = "ltn04098@gmail.com",
                    EmailConfirmed = true,
                    CreatedTime =  DateTime.UtcNow,
                    SecurityStamp = Guid.NewGuid().ToString()
                }
            };
        }

        public static List<Achievement> GetAchievements()
        {
            return new List<Achievement>
            {
               new Achievement
                {
                    Id = Guid.Parse("aaaa1111-1111-1111-1111-111111111111"),
                    Title = "1 Week Smoke-Free",
                    Description = "Reached 7 days without smoking",
                    IconUrl = "https://cdn-icons-png.flaticon.com/512/2921/2921222.png"
                },
                new Achievement
                {
                    Id = Guid.Parse("aaaa2222-2222-2222-2222-222222222222"),
                    Title = "First Quit Plan",
                    Description = "Created a quit plan",
                    IconUrl = "https://cdn-icons-png.flaticon.com/512/1828/1828843.png"
                }
            };
        }

        public static List<UserAchievement> GetUserAchievements()
        {
            return new List<UserAchievement>
            {
                new UserAchievement
                {
                    Id = Guid.NewGuid(),
                    UserId = Guid.Parse("c1c780f9-8dce-41b3-9735-b6bc0e935712"), // John
                    AchievementId = Guid.Parse("aaaa1111-1111-1111-1111-111111111111")
                },
                new UserAchievement
                {
                    Id = Guid.NewGuid(),
                    UserId = Guid.Parse("f77b8d8a-345e-4a63-8928-2ddbdcf7b93b"), // Jane
                    AchievementId = Guid.Parse("aaaa2222-2222-2222-2222-222222222222")
                }
            };
        }
        #region Seeding Blog data
        public static List<Blog> GetBlogs()
        {
            return new List<Blog>
            {
                new Blog
                {
                    Id = Guid.NewGuid(),
                    Title = "My Journey to Quit Smoking",
                    Content = "I started my quit journey this week...",
                    Status = Domain.Enums.BlogStatus.Published,
                    AuthorId = Guid.Parse("c1c780f9-8dce-41b3-9735-b6bc0e935712"),
                     FeaturedImageUrl = "https://images.unsplash.com/photo-1603398938378-329cb7a2391b?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&q=80&w=1080",
                    CreatedTime =  DateTime.UtcNow
                },
                new Blog
                {
                    Id = Guid.NewGuid(),
                    Title = "5 Tips to Beat Cravings",
                    Content = "Drink water, exercise, and avoid triggers...",
                    Status = Domain.Enums.BlogStatus.Published,
                    AuthorId = Guid.Parse("f77b8d8a-345e-4a63-8928-2ddbdcf7b93b"),
                     FeaturedImageUrl = "https://images.unsplash.com/photo-1603398938378-329cb7a2391b?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&q=80&w=1080",
                    CreatedTime =  DateTime.UtcNow
                },
              

                new Blog
                {
                    Id = Guid.NewGuid(),
                    Title = "My Journey to Quit Smoking",
                    Content = "I started my quit journey this week. It’s been challenging but I’m motivated to make a real change for my health and family.",
                    Status = Domain.Enums.BlogStatus.Published,
                    AuthorId = Guid.Parse("c1c780f9-8dce-41b3-9735-b6bc0e935712"),
                     FeaturedImageUrl = "https://images.unsplash.com/photo-1603398938378-329cb7a2391b?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&q=80&w=1080",
                    CreatedTime =  DateTime.UtcNow
                },
                new Blog
                {
                    Id = Guid.NewGuid(),
                    Title = "Day 3 Without a Cigarette",
                    Content = "The cravings are intense, especially after meals. I’m trying to replace the habit with drinking water and chewing gum.",
                    Status = Domain.Enums.BlogStatus.Published,
                    AuthorId = Guid.Parse("c1c780f9-8dce-41b3-9735-b6bc0e935712"),
                     FeaturedImageUrl = "https://images.unsplash.com/photo-1603398938378-329cb7a2391b?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&q=80&w=1080",
                    CreatedTime =  DateTime.UtcNow
                },
                new Blog
                {
                    Id = Guid.NewGuid(),
                    Title = "How I Handle Stress Without Smoking",
                    Content = "I used to smoke every time I felt stressed. Now I go for walks or journal my thoughts. It’s not easy, but it works better over time.",
                    Status = Domain.Enums.BlogStatus.Published,
                    AuthorId = Guid.Parse("c1c780f9-8dce-41b3-9735-b6bc0e935712"),
                     FeaturedImageUrl = "https://images.unsplash.com/photo-1603398938378-329cb7a2391b?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&q=80&w=1080",
                    CreatedTime =  DateTime.UtcNow
                },
                new Blog
                {
                    Id = Guid.NewGuid(),
                    Title = "One Week Smoke-Free!",
                    Content = "I’ve made it through the first week. It’s a small win, but a very meaningful one. I'm breathing easier already.",
                    Status = Domain.Enums.BlogStatus.Published,
                    AuthorId = Guid.Parse("c1c780f9-8dce-41b3-9735-b6bc0e935712"),
                     FeaturedImageUrl = "https://images.unsplash.com/photo-1603398938378-329cb7a2391b?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&q=80&w=1080",
                    CreatedTime =  DateTime.UtcNow
                },
                new Blog
                {
                    Id = Guid.NewGuid(),
                    Title = "Relapsed After 10 Days – What I Learned",
                    Content = "I had a cigarette after 10 days. I felt guilty but I learned that I need a better plan when I’m around other smokers. I'm not giving up.",
                    Status = Domain.Enums.BlogStatus.Published,
                    AuthorId = Guid.Parse("c1c780f9-8dce-41b3-9735-b6bc0e935712"),
                     FeaturedImageUrl = "https://images.unsplash.com/photo-1603398938378-329cb7a2391b?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&q=80&w=1080",

                    CreatedTime =  DateTime.UtcNow
                },
                new Blog
                {
                    Id = Guid.NewGuid(),
                    Title = "The Role of Support in Quitting Smoking",
                    Content = "Having a support system has helped me tremendously. My partner checks in with me every day and I joined an online support group.",
                    Status = Domain.Enums.BlogStatus.Pending_Approval,
                    AuthorId = Guid.Parse("c1c780f9-8dce-41b3-9735-b6bc0e935712"),
                     FeaturedImageUrl = "https://images.unsplash.com/photo-1603398938378-329cb7a2391b?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&q=80&w=1080",
                    CreatedTime =  DateTime.UtcNow
                }
            };

      
        }
        #endregion
    }

}
