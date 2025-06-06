
using AutoMapper.Features;
using Microsoft.AspNetCore.Identity;
using SmokingCessation.Core.Constants;
using SmokingCessation.Core.Utils;
using SmokingCessation.Domain.Entities;
using SmokingCessation.Domain.Enums;
using SmokingCessation.Domain.Interfaces;
using SmokingCessation.Infrastracture.Data.Persistence;

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
                //if (!_dbContext.Blogs.Any())
                //{
                //    _dbContext.Blogs.AddRange(GetBlogs());
                //}
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
            return new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    Id = Guid.Parse("c1c780f9-8dce-41b3-9735-b6bc0e935712"),
                    UserName = "john.doe",
                    FullName = "John Doe",
                    Email = "john.doe@example.com",
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                },
                new ApplicationUser
                {
                    Id = Guid.Parse("f77b8d8a-345e-4a63-8928-2ddbdcf7b93b"),
                    UserName = "jane.smith",
                    FullName = "Jane Smith",
                    Email = "jane.smith@example.com",
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                },
                new ApplicationUser
                {
                    Id = Guid.Parse("75075487-7FA2-4063-9CE7-3C85D2D88A12"),
                    UserName = "quoctrieu15",
                    FullName = "Quoc Trieu",
                    Email = "luongquoctrieu165@gmail.com", // Sửa lỗi định dạng email
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                },
                new ApplicationUser
                {
                    Id = Guid.Parse("BC790193-647D-420F-BC27-C8ABB27BCB53"),
                    UserName = "ThanhVu22",
                    FullName = "Lê Thanh Vũ",
                    Email = "vult2911@gmail.com",
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                },
                new ApplicationUser
                {
                    Id = Guid.Parse("A0CBD7C7-D6A3-4912-B8C7-3029B55B5E3D"),
                    UserName = "VietQuoc01",
                    FullName = "Nguyễn Trần Việt Quốc",
                    Email = "ntvq88@gmail.com",
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                },
                new ApplicationUser
                {
                    Id = Guid.Parse("4BD6B6BC-58E9-4A2A-B1CE-3FF3F11D5D4C"),
                    UserName = "NhatTruong02",
                    FullName = "Lê Nhật Trường",
                    Email = "ltn04098@gmail.com",
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
                
                    Id = Guid.Parse("365B7E24-1CB1-4128-8D63-7C60604CD977"),
                    Title = "1 Week Smoke-Free",
                    Description = "Reached 7 days without smoking",
                    IconUrl = "https://cdn-icons-png.flaticon.com/512/2921/2921222.png"
                },
                new Achievement
                {
               
                    Id = Guid.Parse("AB1FE570-A021-4541-8553-A4AF695D73B8"),
                    Title = "First Quit Plan",
                    Description = "Created a quit plan",
                    IconUrl = "https://cdn-icons-png.flaticon.com/512/1828/1828843.png"
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
                CreatedTime = CoreHelper.SystemTimeNow,
                LastUpdatedTime = CoreHelper.SystemTimeNow
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
                    CreatedTime = CoreHelper.SystemTimeNow,
                    LastUpdatedTime = CoreHelper.SystemTimeNow
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
                    CreatedTime = CoreHelper.SystemTimeNow,
                    LastUpdatedTime = CoreHelper.SystemTimeNow
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
                    CreatedTime = CoreHelper.SystemTimeNow,
                    LastUpdatedTime = CoreHelper.SystemTimeNow
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
                    CreatedTime = DateTime.UtcNow
                },
                new Blog
                {
                    Id = Guid.NewGuid(),
                    Title = "5 Tips to Beat Cravings",
                    Content = "Drink water, exercise, and avoid triggers...",
                    AuthorId = Guid.Parse("f77b8d8a-345e-4a63-8928-2ddbdcf7b93b"),
                    CreatedTime = DateTime.UtcNow
                }
            };
        }
    }

}
