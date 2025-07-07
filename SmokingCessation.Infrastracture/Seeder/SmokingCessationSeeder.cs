
using AutoMapper.Features;
using Microsoft.AspNetCore.Identity;
using SmokingCessation.Core.Constants;
using SmokingCessation.Core.Utils;
using SmokingCessation.Domain.Entities;
using SmokingCessation.Domain.Enums;
using SmokingCessation.Domain.Interfaces;
using SmokingCessation.Infrastracture.Data.Persistence;
using static System.Net.WebRequestMethods;

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
                if (!_dbContext.Blogs.Any())
                {
                    _dbContext.Blogs.AddRange(GetBlogs());
                }
                if (!_dbContext.Roles.Any())
                {
                    var roles = GetRoles();
                    _dbContext.Roles.AddRange(roles);

                }
                if (!_dbContext.MembershipPackages.Any()) // Giả sử bạn có DbSet<MembershipPackage> MembershipPackages trong DbContext
                {
                    _dbContext.MembershipPackages.AddRange(GetMembershipPackages());
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
            var hasher = new PasswordHasher<ApplicationUser>();
            return new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    Id = Guid.Parse("c1c780f9-8dce-41b3-9735-b6bc0e935712"),
                    UserName = "john.doe",
                    FullName = "John Doe",
                    ImageUrl = "https://hoseiki.vn/wp-content/uploads/2025/03/avatar-mac-dinh-20.jpg",
                   
                    PasswordHash = hasher.HashPassword(null, "Ab@123456"),
                    Email = "john.doe@example.com",
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                },
                new ApplicationUser
                {
                    Id = Guid.Parse("f77b8d8a-345e-4a63-8928-2ddbdcf7b93b"),
                    UserName = "jane.smith",
                    NormalizedUserName = "jane.smith",
                    FullName = "Jane Smith",
                    ImageUrl = "https://hoseiki.vn/wp-content/uploads/2025/03/avatar-mac-dinh-20.jpg",

                    PasswordHash = hasher.HashPassword(null, "Ab@123456"),
                    Email = "jane.smith@example.com",
                    NormalizedEmail = "jane.smith@example.com",
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                },
                new ApplicationUser
                {
                    Id = Guid.Parse("75075487-7FA2-4063-9CE7-3C85D2D88A12"),
                    UserName = "quoctrieu15",
                    NormalizedUserName = "quoctrieu15",
                    FullName = "Quoc Trieu",
                    Email = "luongquoctrieu165@gmail.com", // Sửa lỗi định dạng email
                    NormalizedEmail ="luongquoctrieu165@gmail.com",
                    ImageUrl = "https://hoseiki.vn/wp-content/uploads/2025/03/avatar-mac-dinh-20.jpg",

                    PasswordHash = hasher.HashPassword(null, "Ab@123456"),
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                },
                new ApplicationUser
                {
                    Id = Guid.Parse("BC790193-647D-420F-BC27-C8ABB27BCB53"),
                    UserName = "ThanhVu22",
                    NormalizedUserName = "ThanhVu22",
                    FullName = "Lê Thanh Vũ",
                    Email = "vult2911@gmail.com",
                    NormalizedEmail = "vult2911@gmail.com",
                    ImageUrl = "https://hoseiki.vn/wp-content/uploads/2025/03/avatar-mac-dinh-20.jpg",

                    PasswordHash = hasher.HashPassword(null, "Ab@123456"),
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                },
                new ApplicationUser
                {
                    Id = Guid.Parse("A0CBD7C7-D6A3-4912-B8C7-3029B55B5E3D"),
                    UserName = "VietQuoc01",
                    NormalizedUserName = "VietQuoc01",
                    FullName = "Nguyễn Trần Việt Quốc",
                    Email = "ntvq88@gmail.com",
                    NormalizedEmail = "ntvq88@gmail.com",
                    ImageUrl = "https://hoseiki.vn/wp-content/uploads/2025/03/avatar-mac-dinh-20.jpg",
                    PasswordHash = hasher.HashPassword(null, "Ab@123456"),
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow,

                },
                new ApplicationUser
                {
                    Id = Guid.Parse("4BD6B6BC-58E9-4A2A-B1CE-3FF3F11D5D4C"),
                    UserName = "NhatTruong02",
                    NormalizedUserName = "NhatTruong02",
                    FullName = "Lê Nhật Trường",
                    Email = "ltn04098@gmail.com",
                    NormalizedEmail = "ltn04098@gmail.com",
                    ImageUrl = "https://hoseiki.vn/wp-content/uploads/2025/03/avatar-mac-dinh-20.jpg",

                    PasswordHash = hasher.HashPassword(null, "Ab@123456"),
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                }
            };
        }

        public static List<Achievement> GetAchievements()
        {
            return new List<Achievement>
            {
                      new Achievement
                {
                         
                    Id = Guid.Parse("D7ECC0D7-08AD-4B55-89BD-49260A169BAB"),
                    Title = "Set Quit Date",
                    Description = "You have set your quit date. Your journey begins!",
                    IconUrl = "https://res.cloudinary.com/dae7xasfc/image/upload/v1750950458/SetQuitDate_n7dl5a.png",
                    ConditionType = ConditionType.SetQuitDate,
                    ConditionValue = 1,
                    CreatedBy = "75075487-7FA2-4063-9CE7-3C85D2D88A12",
                    CreatedTime =  DateTime.UtcNow,
                    LastUpdatedBy =  "75075487-7FA2-4063-9CE7-3C85D2D88A12",
                    LastUpdatedTime =  DateTime.UtcNow
                },
                new Achievement
                {
                    Id = Guid.Parse("843cb5c1-4351-4f62-8117-cb7d99e08d80"),
                    Title = "Superhero",
                    Description = "Stay smoke-free for 7 consecutive days.",
                    IconUrl = "https://res.cloudinary.com/dae7xasfc/image/upload/v1750950458/SupperHero_hfx2uv.png",
                    ConditionType = ConditionType.DaysSmokeFree,
                    ConditionValue = 7,
                     CreatedBy = "75075487-7FA2-4063-9CE7-3C85D2D88A12",
                    CreatedTime =  DateTime.UtcNow,
                    LastUpdatedBy =  "75075487-7FA2-4063-9CE7-3C85D2D88A12",
                    LastUpdatedTime = DateTime.UtcNow
                },
                new Achievement
                {
                    Id = Guid.Parse("fd2030dd-7660-4627-a291-b50463a21fbd"),
                    Title = "Community Leader",
                    Description = "Post 5 comments or messages to support others.",
                    IconUrl = "https://res.cloudinary.com/dae7xasfc/image/upload/v1750950459/communityLeader_j2uqrt.png",
                    ConditionType = ConditionType.CommentsPosted,
                    ConditionValue = 5,
                     CreatedBy = "75075487-7FA2-4063-9CE7-3C85D2D88A12",
                    CreatedTime = DateTime.UtcNow,
                    LastUpdatedBy =  "75075487-7FA2-4063-9CE7-3C85D2D88A12",
                    LastUpdatedTime = DateTime.UtcNow
                },
                new Achievement
                {
                    Id = Guid.Parse("e29a85da-1bd0-460b-a1f8-66d3aacf8830"),
                    Title = "Gums & Teeth",
                    Description = "Stay smoke-free for 14 days and smile brighter!",
                    IconUrl = "https://res.cloudinary.com/dae7xasfc/image/upload/v1750950458/Gum_Teeth_ayncnf.png",
                    ConditionType = ConditionType.DaysSmokeFree,
                    ConditionValue = 14,
                     CreatedBy = "75075487-7FA2-4063-9CE7-3C85D2D88A12",
                    CreatedTime = DateTime.UtcNow, 
                    LastUpdatedBy =  "75075487-7FA2-4063-9CE7-3C85D2D88A12",
                    LastUpdatedTime = DateTime.UtcNow
                },
                new Achievement
                {
                    Id = Guid.Parse("baf2ed29-2ae3-461c-85ef-49fa57b6bec2"),
                    Title = "Explorer",
                    Description = "Try 3 different features of the app.",
                    IconUrl = "https://res.cloudinary.com/dae7xasfc/image/upload/v1750950458/Explore_e4giw7.png",
                    ConditionType = ConditionType.FeaturesUsed,
                    ConditionValue = 3,
                     CreatedBy = "75075487-7FA2-4063-9CE7-3C85D2D88A12",
                    CreatedTime = DateTime.UtcNow,
                    LastUpdatedBy =  "75075487-7FA2-4063-9CE7-3C85D2D88A12",
                    LastUpdatedTime = DateTime.UtcNow
                },
                new Achievement
                {
                    Id = Guid.Parse("2b1f0897-f3f1-4517-9198-3668c37ee64c"),
                    Title = "25 Missions Completed",
                    Description = "You've completed 25 tasks. Keep going!",
                    IconUrl = "https://res.cloudinary.com/dae7xasfc/image/upload/v1750950458/missionComplete_udtcn2.png",
                    ConditionType = ConditionType.MissionsCompleted,
                    ConditionValue = 25,
                     CreatedBy = "75075487-7FA2-4063-9CE7-3C85D2D88A12",
                    CreatedTime = DateTime.UtcNow,
                    LastUpdatedBy =  "75075487-7FA2-4063-9CE7-3C85D2D88A12",
                    LastUpdatedTime = DateTime.UtcNow
                },
                new Achievement
                {
                    Id = Guid.Parse("26e8d8e2-caf0-4b29-84c0-3fcbe494027b"),
                    Title = "Diary Writer",
                    Description = "Write 10 journal entries to track your journey.",
                    IconUrl = "https://res.cloudinary.com/dae7xasfc/image/upload/v1750950458/diary_entries_xktwgu.png",
                    ConditionType = ConditionType.DiaryEntries,
                    ConditionValue = 10,
                     CreatedBy = "75075487-7FA2-4063-9CE7-3C85D2D88A12",
                    CreatedTime = DateTime.UtcNow,
                    LastUpdatedBy =  "75075487-7FA2-4063-9CE7-3C85D2D88A12",
                    LastUpdatedTime = DateTime.UtcNow
                },
                new Achievement
                {
                    Id = Guid.Parse("b44c860e-e888-4027-8721-1db5cf39d49f"),
                    Title = "1 Month Smoke-Free",
                    Description = "You’ve gone a full month without smoking!",
                    IconUrl = "https://res.cloudinary.com/dae7xasfc/image/upload/v1750950459/1_month_smoke_free_gz4lwz.png",
                    ConditionType = ConditionType.DaysSmokeFree,
                    ConditionValue = 30,
                     CreatedBy = "75075487-7FA2-4063-9CE7-3C85D2D88A12",
                    CreatedTime = DateTime.UtcNow,
                    LastUpdatedBy =  "75075487-7FA2-4063-9CE7-3C85D2D88A12",
                    LastUpdatedTime = DateTime.UtcNow
                },
                new Achievement
                {
                    Id = Guid.Parse("fde1b6d5-5f44-4b67-8389-70b3ac973067"),
                    Title = "Money Saved",
                    Description = "Saved over 1,000,000 VND from quitting smoking.",
                    IconUrl = "https://res.cloudinary.com/dae7xasfc/image/upload/v1750950458/money_saved_f63oob.png",
                    ConditionType = ConditionType.MoneySaved,
                    ConditionValue = 1000000,
                     CreatedBy = "75075487-7FA2-4063-9CE7-3C85D2D88A12",
                    CreatedTime = DateTime.UtcNow,
                    LastUpdatedBy =  "75075487-7FA2-4063-9CE7-3C85D2D88A12",
                    LastUpdatedTime = DateTime.UtcNow
                },
                new Achievement
                {
                    Id = Guid.Parse("caafcc33-64fa-4023-ac21-d01c9f049110"),
                    Title = "Cravings Resisted",
                    Description = "Resisted the urge to smoke 25 times.",
                    IconUrl = "https://res.cloudinary.com/dae7xasfc/image/upload/v1750950459/cravings_resisted_rizsac.png",
                    ConditionType = ConditionType.CravingsResisted,
                    ConditionValue = 25,
                    CreatedBy = "75075487-7FA2-4063-9CE7-3C85D2D88A12",
                    CreatedTime = DateTime.UtcNow,
                    LastUpdatedBy =  "75075487-7FA2-4063-9CE7-3C85D2D88A12",
                    LastUpdatedTime = DateTime.UtcNow
                }
            };
        }

        public static List<MembershipPackage> GetMembershipPackages()
        {
            return new List<MembershipPackage> {
                new MembershipPackage
            {

                Id = Guid.Parse("C9ABEC75-07BB-4C29-8EC3-95939DC3D548"), // Sử dụng Guid duy nhất cho ID
                Name = "Gói Miễn Phí",
                Price = 0.00m,
                Type = MembershipType.Free,
                Description = "Gói miễn phí với các tính năng cơ bản như theo dõi tiến độ và nhiệm vụ hàng ngày.",
                DurationMonths = 0, // Gói Free không có thời hạn cố định, hoặc là 0 tháng
                Features = "Theo dõi tiến độ, nhiệm vụ cơ bản, nhật ký hút thuốc.",
                CreatedTime =  DateTime.UtcNow,
                LastUpdatedTime =  DateTime.UtcNow
                // CreatedBy, LastUpdatedBy, DeletedBy, DeletedTime có thể để null
            },
                new MembershipPackage
                {
                    Id = Guid.Parse("3BF04C57-E0F9-46AB-BC32-03F5A12759CB"),
                    Name = "Gói Cơ Bản",
                    Price = 50000.00m, // Ví dụ giá
                    Type = MembershipType.Basic,
                    Description = "Gói cơ bản bao gồm các tính năng nâng cao và tài nguyên hỗ trợ.",
                    DurationMonths = 1, // Gói Basic kéo dài 1 tháng
                    Features = "Tất cả của Gói Miễn Phí, thêm: Kế hoạch cai thuốc cá nhân hóa, báo cáo tiến độ chi tiết, 10 nhiệm vụ hàng ngày.",
                    CreatedTime =  DateTime.UtcNow,
                    LastUpdatedTime =  DateTime.UtcNow
                },
                new MembershipPackage
                {
                    Id = Guid.Parse("BE990A1B-8667-4421-9FEA-69CFEB6E1E49"),
                    Name = "Gói Cao Cấp",
                    Price = 150000.00m, // Ví dụ giá
                    Type = MembershipType.Premium,
                    Description = "Gói cao cấp với tất cả tính năng, hỗ trợ cá nhân và quyền truy cập độc quyền.",
                    DurationMonths = 3, // Gói Premium kéo dài 3 tháng
                    Features = "Tất cả của Gói Cơ Bản, thêm: Tư vấn 1-1 với chuyên gia (2 buổi/tháng), truy cập thư viện video độc quyền, ưu tiên hỗ trợ, 20 nhiệm vụ hàng ngày.",
                    CreatedTime =  DateTime.UtcNow,
                    LastUpdatedTime =  DateTime.UtcNow
                },
                new MembershipPackage
                {

                    Id = Guid.Parse("8F0467C6-F75B-4F66-B787-EA8BE3AA509B"),
                    Name = "Gói Doanh Nghiệp",
                    Price = 500000.00m, // Ví dụ giá
                    Type = MembershipType.Enterprise,
                    Description = "Gói dành cho doanh nghiệp, cung cấp giải pháp cai thuốc cho nhân viên.",
                    DurationMonths = 12, // Gói Enterprise kéo dài 12 tháng
                    Features = "Tất cả của Gói Cao Cấp, thêm: Bảng điều khiển quản lý nhóm, báo cáo tổng hợp, chương trình sức khỏe tùy chỉnh, hỗ trợ triển khai tại công ty.",
                    CreatedTime =  DateTime.UtcNow,
                    LastUpdatedTime =  DateTime.UtcNow
                }
            };
        }



        public static List<Blog> GetBlogs()
        {
            return new List<Blog>
            {
                new Blog
                {
                    Id = Guid.NewGuid(),
                    Title = "My Journey to Quit Smoking",
                    Content = "I started my quit journey this week...",
                    AuthorId = Guid.Parse("c1c780f9-8dce-41b3-9735-b6bc0e935712"),
                    Status = BlogStatus.Published,
                    FeaturedImageUrl= "https://res.cloudinary.com/dae7xasfc/image/upload/v1750146369/enhcnhezb9eqimew7k4p.jpg",
                    CreatedTime =  DateTime.UtcNow
                },
                new Blog
                {
                    Id = Guid.NewGuid(),
                    Title = "5 Tips to Beat Cravings",
                    Content = "Drink water, exercise, and avoid triggers...",
                    AuthorId = Guid.Parse("f77b8d8a-345e-4a63-8928-2ddbdcf7b93b"),
                    FeaturedImageUrl= "https://res.cloudinary.com/dae7xasfc/image/upload/v1750146369/enhcnhezb9eqimew7k4p.jpg",
                    CreatedTime =  DateTime.UtcNow
                },
                new Blog
                {
                    Id = Guid.NewGuid(),
                    Title = "5 Tips to Beat Cravings",
                    Content = "Drink water, exercise, and avoid triggers...",
                    Status = Domain.Enums.BlogStatus.Published,
                    AuthorId = Guid.Parse("f77b8d8a-345e-4a63-8928-2ddbdcf7b93b"),
                    CreatedTime =  DateTime.UtcNow,
                    FeaturedImageUrl= "https://res.cloudinary.com/dae7xasfc/image/upload/v1750146369/enhcnhezb9eqimew7k4p.jpg",
                },
                new Blog
                {
                    Id = Guid.NewGuid(),
                    Title = "Day 3 Without a Cigarette",
                    Content = "The cravings are intense...",
                    Status = Domain.Enums.BlogStatus.Published,
                    AuthorId = Guid.Parse("c1c780f9-8dce-41b3-9735-b6bc0e935712"),
                    CreatedTime =  DateTime.UtcNow,
                    FeaturedImageUrl= "https://res.cloudinary.com/dae7xasfc/image/upload/v1750146369/enhcnhezb9eqimew7k4p.jpg",
                },
                new Blog
                {
                    Id = Guid.NewGuid(),
                    Title = "One Week Smoke-Free!",
                    Content = "I’ve made it through the first week...",
                    Status = Domain.Enums.BlogStatus.Published,
                    AuthorId = Guid.Parse("c1c780f9-8dce-41b3-9735-b6bc0e935712"),
                    CreatedTime =  DateTime.UtcNow,
                    FeaturedImageUrl= "https://res.cloudinary.com/dae7xasfc/image/upload/v1750146369/enhcnhezb9eqimew7k4p.jpg",
                },
                new Blog
                {
                    Id = Guid.NewGuid(),
                    Title = "Relapsed After 10 Days – What I Learned",
                    Content = "I had a cigarette after 10 days...",
                    Status = Domain.Enums.BlogStatus.Published,
                    AuthorId = Guid.Parse("c1c780f9-8dce-41b3-9735-b6bc0e935712"),
                    CreatedTime =  DateTime.UtcNow,
                    FeaturedImageUrl= "https://res.cloudinary.com/dae7xasfc/image/upload/v1750146369/enhcnhezb9eqimew7k4p.jpg",
                },
                new Blog
                {
                    Id = Guid.NewGuid(),
                    Title = "The Role of Support in Quitting Smoking",
                    Content = "Having a support system has helped me tremendously...",
                    Status = Domain.Enums.BlogStatus.Pending_Approval,
                    AuthorId = Guid.Parse("c1c780f9-8dce-41b3-9735-b6bc0e935712"),
                    CreatedTime =  DateTime.UtcNow,
                    FeaturedImageUrl= "https://res.cloudinary.com/dae7xasfc/image/upload/v1750146369/enhcnhezb9eqimew7k4p.jpg",
                }
            };
        }
    }

}
