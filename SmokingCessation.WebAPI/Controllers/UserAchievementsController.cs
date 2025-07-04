using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmokingCessation.Application.DTOs.Response;
using SmokingCessation.Application.Service.Interface;
using SmokingCessation.Core.Response;

namespace SmokingCessation.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAchievementsController : ControllerBase
    {
        private readonly IUserAchievementService _userAchievementService;

        public UserAchievementsController(IUserAchievementService userAchievementService)
        {
            _userAchievementService = userAchievementService;
        }

        // GET: api/UserAchievements/{userId}
        [HttpGet("{userId}")]
        public async Task<ActionResult<BaseResponseModel<UserAchivementResponse>>> GetUserAchievements(Guid userId)
        {
            var response = await _userAchievementService.GetUserAchievementsAsync(userId);
            return Ok(response);
        }
        // POST: api/UserAchievements/assign/{userId}
        /// <summary>
        /// Assigns achievements to a user if they are eligible based on their progress and other criteria.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost("assign/{userId}")]
        public async Task<IActionResult> AssignAchievements(Guid userId)
        {
            await _userAchievementService.AssignAchievementsIfEligibleAsync(userId);
            return NoContent();
        }
        // GET: api/UserAchievements/has-achievement/{userId}/{achievementId}
        [HttpGet("has-achievement/{userId}/{achievementId}")]
        public async Task<ActionResult<bool>> HasAchievement(Guid userId, Guid achievementId)
        {
            var hasAchievement = await _userAchievementService.HasAchievementAsync(userId, achievementId);
            return Ok(hasAchievement);
        }
    }
}
