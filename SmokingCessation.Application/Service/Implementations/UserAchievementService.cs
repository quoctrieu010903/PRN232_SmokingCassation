using SmokingCessation.Application.Service.Interface;
using SmokingCessation.Domain.Entities;
using SmokingCessation.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmokingCessation.Application.Service.Implementations
{
    public class UserAchievementService 
    {
        
        private readonly IUnitOfWork _unitOfWork;

        public UserAchievementService( IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AssignAchievementAsync(int userId, int achievementId)
        {
          throw new NotImplementedException();
        }

        public async Task<List<int>> GetUserAchievementsAsync(int userId)
        {
            throw new NotImplementedException();

        }

        public async Task<bool> HasAchievementAsync(int userId, int achievementId)
        {
            throw new NotImplementedException();

        }
    }
} 