using SmokingCessation.Application.DTOs.Response;
using SmokingCessation.Core.Response;
using SmokingCessation.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmokingCessation.Application.Service.Interface
{
    public interface IUserAchievementService
    {
        Task AssignAchievementsIfEligibleAsync(Guid userId);
        Task<bool> HasAchievementAsync(Guid userId, Guid achievementId);
        Task<BaseResponseModel<List<UserAchivementResponse>>> GetUserAchievementsAsync(Guid userId);
    }
} 