using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SmokingCessation.Application.DTOs.Response;
using SmokingCessation.Application.Service.Interface;
using SmokingCessation.Core.Constants;
using SmokingCessation.Core.Response;
using SmokingCessation.Domain.Entities;
using SmokingCessation.Domain.Interfaces;
using SmokingCessation.Domain.Specifications;

namespace SmokingCessation.Application.Service.Implementations
{
    public class UserDashboardService : IUserDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserDashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<BaseResponseModel<UserDashboardDto>> GetUserStatisticsAsync(Guid userId, DateTime? week = null, DateTime? month = null)
        {
            var now = DateTime.UtcNow;
            var weekStart = (week ?? now).Date.AddDays(-(int)(week ?? now).DayOfWeek + (int)DayOfWeek.Monday);
            var weekEnd = weekStart.AddDays(6);

            var monthDate = month ?? now;
            var monthStart = new DateTime(monthDate.Year, monthDate.Month, 1);
            var monthEnd = monthStart.AddMonths(1).AddDays(-1);

            var progressLogRepo = _unitOfWork.Repository<ProgressLog, ProgressLog>();
            var logs = await progressLogRepo.GetAllWithSpecAsync(
                new BaseSpecification<ProgressLog>(p => p.QuitPlan.UserId == userId), true);

            // Daily counts for week
            var dailyCountsThisWeek = logs
                .Where(l => l.LogDate.Date >= weekStart && l.LogDate.Date <= weekEnd)
                .GroupBy(l => l.LogDate.Date)
                .Select(g => new DailyCigaretteCountDto
                {
                    Date = g.Key,
                    CigarettesSmoked = g.Sum(x => x.SmokedToday)
                })
                .OrderBy(x => x.Date)
                .ToList();

            // Daily counts for month
            var dailyCountsThisMonth = logs
                .Where(l => l.LogDate.Date >= monthStart && l.LogDate.Date <= monthEnd)
                .GroupBy(l => l.LogDate.Date)
                .Select(g => new DailyCigaretteCountDto
                {
                    Date = g.Key,
                    CigarettesSmoked = g.Sum(x => x.SmokedToday)
                })
                .OrderBy(x => x.Date)
                .ToList();

            var cigarettesThisWeek = dailyCountsThisWeek.Sum(x => x.CigarettesSmoked);
            var cigarettesThisMonth = dailyCountsThisMonth.Sum(x => x.CigarettesSmoked);
            var totalSmokeFreeDays = logs.Count(l => l.SmokedToday == 0);

            var dashboard = new UserDashboardDto
            {
                CigarettesThisWeek = cigarettesThisWeek,
                CigarettesThisMonth = cigarettesThisMonth,
                TotalSmokeFreeDays = totalSmokeFreeDays,
                DailyCountsThisWeek = dailyCountsThisWeek,
                DailyCountsThisMonth = dailyCountsThisMonth
            };

            return new  BaseResponseModel<UserDashboardDto>(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, dashboard);

        }
    }
}
