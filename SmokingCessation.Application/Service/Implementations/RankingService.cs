using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmokingCessation.Application.DTOs.Response;
using SmokingCessation.Application.Service.Interface;
using SmokingCessation.Core.Response;
using SmokingCessation.Domain.Entities;
using SmokingCessation.Domain.Interfaces;
using SmokingCessation.Domain.Specifications;

namespace SmokingCessation.Application.Service.Implementations
{
    public class RankingService : IRankingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private const decimal MoneySavedPerDay = 15000;

        public RankingService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponseModel<IEnumerable<UserRankingDetailDto>>> GetUserRankingsWithDetailsAsync()
        {
            // Get all users (excluding deleted)
            var users = await _unitOfWork.Repository<ApplicationUser, ApplicationUser>().GetAllWithIncludeAsync(true,
                u => u.QuitPlans,
                u => u.ProgressLogs,
                u => u.UserAchievements

            );

            var rankingList = new List<UserRankingDetailDto>();

            foreach (var user in users.Where(u => u.DeletedTime == null))
            {
                foreach (var ua in user.UserAchievements)
                {
                    if (ua.Achievement == null)
                    {
                        ua.Achievement = await _unitOfWork.Repository<Achievement,Achievement>().GetWithSpecAsync(new BaseSpecification<Achievement>((a => a.Id == ua.AchievementId)),true);
                    }
                }
                // Get all quit plans for user
                var quitPlans = user.QuitPlans?.Where(qp => qp != null).ToList() ?? new List<QuitPlan>();
                // Get all progress logs for user (via quit plans)
                var progressLogs = quitPlans.SelectMany(qp => qp.ProgressLogs ?? new List<ProgressLog>()).ToList();
                // Count smoke-free days
                int totalSmokeFreeDays = progressLogs.Count(pl => pl.SmokedToday == 0);
                // Count achievements
                int achievementCount = user.UserAchievements?.Count(ua => ua != null) ?? 0;
                // Calculate money saved
                decimal totalMoneySaved = totalSmokeFreeDays * MoneySavedPerDay;
                var badges = user.UserAchievements?
                            .Where(ua => ua.Achievement != null)
                            .Select(ua => new BadgeDto
                            {
                                Title = ua.Achievement.Title,   // bạn có thể dùng `Name` nếu không có `Title`
                                IconUrl = ua.Achievement.IconUrl
                            })
                            .ToList() ?? new List<BadgeDto>();


                rankingList.Add(new UserRankingDetailDto
                {
                    UserId = user.Id,
                    FullName = user.FullName,
                    AchievementCount = achievementCount,
                    TotalSmokeFreeDays = totalSmokeFreeDays,
                    TotalMoneySaved = totalMoneySaved,
                    Badges = badges,
                    RelapseRisk = "Dữ liệu không đủ"
                });
            }

            // Order by TotalSmokeFreeDays desc, then AchievementCount desc
            var ordered = rankingList
                .OrderByDescending(r => r.TotalSmokeFreeDays)
                .ThenByDescending(r => r.AchievementCount)
                .ToList();

            // Assign rank
            int rank = 1;
            foreach (var item in ordered)
            {
                item.Rank = rank++;
            }

            return new BaseResponseModel<IEnumerable<UserRankingDetailDto>>(
                200,
                "SUCCESS",
                ordered,
                "User rankings fetched successfully."
            );
        }
    }
}
