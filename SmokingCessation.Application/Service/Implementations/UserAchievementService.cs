using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SmokingCessation.Application.DTOs.Response;
using SmokingCessation.Application.Service.Interface;
using SmokingCessation.Core.Constants;
using SmokingCessation.Core.Response;
using SmokingCessation.Domain.Entities;
using SmokingCessation.Domain.Enums;
using SmokingCessation.Domain.Interfaces;
using SmokingCessation.Domain.Specifications;
using SmokingCessation.Infrastracture.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmokingCessation.Application.Service.Implementations
{
    public class UserAchievementService : IUserAchievementService
    {
        
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _Imapper;

        public UserAchievementService( IUnitOfWork unitOfWork ,IMapper mapper )
        {
            _unitOfWork = unitOfWork;
            _Imapper = mapper;
        }

        public async Task AssignAchievementsIfEligibleAsync(Guid userId)
        {
            // L?y danh sách AchievementId mà user ?ã có
            var userAchievementRepo = _unitOfWork.Repository<UserAchievement, UserAchievement>();
            var achievementRepo = _unitOfWork.Repository<Achievement, Achievement>();

            var userAchievements = await userAchievementRepo
                .GetAllWithSpecAsync(new BaseSpecification<UserAchievement>(x => x.UserId == userId),  true);
            var ownedAchievementIds = userAchievements.Select(x => x.AchievementId).ToHashSet();

            // L?y t?t c? achievement h? th?ng
            var achievements = await achievementRepo.GetAllAsync();

            foreach (var achievement in achievements)
            {
                if (ownedAchievementIds.Contains(achievement.Id)) continue;

                var met = await CheckConditionAsync(userId, achievement.ConditionType, achievement.ConditionValue);
                if (met)
                {
                    var newUserAchievement = new UserAchievement
                    {
                        UserId = userId,
                        AchievementId = achievement.Id,
                        GrantedAt = DateTime.UtcNow,
                        CreatedTime = DateTime.UtcNow, 
                    };

                    await userAchievementRepo.AddAsync(newUserAchievement);
                }
            }

            await _unitOfWork.SaveChangesAsync();
        }


        public async Task<BaseResponseModel<List<UserAchivementResponse>>> GetUserAchievementsAsync(Guid userId)
        {

            var userAchievementRepo = _unitOfWork.Repository<UserAchievement, UserAchievement>();
            var spec = new BaseSpecification<UserAchievement>(x => x.UserId == userId);
          var result = await userAchievementRepo.GetAllWithSpecWithInclueAsync(spec,true,p => p.User , p => p.Achievement);
            var response = _Imapper.Map<List<UserAchivementResponse>>(result);

            return new BaseResponseModel<List<UserAchivementResponse>>(
                StatusCodes.Status200OK,
                ResponseCodeConstants.SUCCESS,
                response
            );


        }

        public Task<bool> HasAchievementAsync(Guid userId, Guid achievementId)
        {
            throw new NotImplementedException();
        }

        private async Task<bool> CheckConditionAsync(Guid userId, ConditionType type, int value)
        {
            switch ((ConditionType)type)
            {
                case ConditionType.SetQuitDate:
                    return await _unitOfWork.Repository<QuitPlan,QuitPlan>().AnyAsync(q => q.UserId == userId);

                case ConditionType.DaysSmokeFree:
                    var spec = new BaseSpecification<ProgressLog>(q =>
                        q.QuitPlan.UserId == userId && q.SmokedToday == 0);

                    var smokeFreeDays = await _unitOfWork.Repository<ProgressLog, ProgressLog>().CountAsync(spec);


                    return smokeFreeDays >= value;

                case ConditionType.CommentsPosted:
                    var comments = await _unitOfWork.Repository<Feedback, Feedback>().CountAsync(new BaseSpecification<Feedback>(f => f.UserId == userId));
                    return comments >= value;

                case ConditionType.FeaturesUsed:
                    var featureCount = await CountUsedFeatures(userId);
                    return featureCount >= value;

                case ConditionType.MissionsCompleted:
                case ConditionType.DiaryEntries:
                    var diaries = await _unitOfWork.Repository<ProgressLog, ProgressLog>().CountAsync(new BaseSpecification<ProgressLog>(f=>f.QuitPlan.UserId == userId));
                    return diaries >= value;

                case ConditionType.MoneySaved:
                    // L?y QuitPlan c?a user
                    var quitPlanSpec = new BaseSpecification<QuitPlan>(q => q.UserId == userId);
                    var quitPlan = await _unitOfWork.Repository<QuitPlan, QuitPlan>()
                        .GetWithSpecAsync(quitPlanSpec);

                    if (quitPlan == null) return false;

                    // L?c ProgressLog theo:
                    // - SmokedToday == 0
                    // - LogDate trong kho?ng StartDate - TargetDate
                    var progressSpec = new BaseSpecification<ProgressLog>(p =>
                        p.QuitPlan.UserId == userId &&
                        p.SmokedToday == 0 &&
                        p.LogDate.Date >= quitPlan.StartDate.Date &&
                        p.LogDate.Date <= quitPlan.TargetDate.Date);

                     smokeFreeDays = await _unitOfWork.Repository<ProgressLog, ProgressLog>()
                        .CountAsync(progressSpec);

                    var moneySaved = smokeFreeDays * 15000;
                    return moneySaved >= value;

                case ConditionType.CravingsResisted:
                    var cravingLogs = await _unitOfWork.Repository<ProgressLog, ProgressLog>()
                         .CountAsync(new BaseSpecification<ProgressLog>(p =>
                             p.QuitPlan.UserId == userId &&
                             p.Note != null &&
                             p.Note.ToLower().Contains("v??t qua")));
                    return cravingLogs >= value;

                default:
                    return false;
            }
        }

        private async Task<int> CountUsedFeatures(Guid userId)
        {
            int total = 0;

            var progressLogSpec = new BaseSpecification<ProgressLog>(f => f.QuitPlan.UserId == userId);
            if (await _unitOfWork.Repository<ProgressLog, ProgressLog>().AnyAsync(progressLogSpec))
                total++;

            var feedbackSpec = new BaseSpecification<Feedback>(f => f.UserId == userId);
            if (await _unitOfWork.Repository<Feedback, Feedback>().AnyAsync(feedbackSpec))
                total++;

            var ratingSpec = new BaseSpecification<Rating>(r => r.UserId == userId);
            if (await _unitOfWork.Repository<Rating, Rating>().AnyAsync(ratingSpec))
                total++;

            var coachAdviceSpec = new BaseSpecification<CoachAdviceLog>(c => c.QuitPlan.UserId == userId);
            if (await _unitOfWork.Repository<CoachAdviceLog, CoachAdviceLog>().AnyAsync(coachAdviceSpec))
                total++;

            var logSpec = new BaseSpecification<ProgressLog>(p => p.QuitPlan.UserId == userId);
            if (await _unitOfWork.Repository<ProgressLog, ProgressLog>().AnyAsync(logSpec))
                total++;
            return total;
        }
    }
} 