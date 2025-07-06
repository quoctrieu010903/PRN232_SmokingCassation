using Microsoft.AspNetCore.Mvc;
using SmokingCessation.Application.Service.Interface;
using SmokingCessation.Application.DTOs.Response;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SmokingCessation.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RankingController : ControllerBase
    {
        private readonly IRankingService _rankingService;

        public RankingController(IRankingService rankingService)
        {
            _rankingService = rankingService;
        }

        /// <summary>
        /// Get all user rankings
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllRankings()
        {
            var result = await _rankingService.GetUserRankingsWithDetailsAsync();
            return Ok(result);
        }

        /// <summary>
        /// Get top N user rankings
        /// </summary>
        [HttpGet("top/{count}")]
        public async Task<IActionResult> GetTopRankings(int count)
        {
            var result = await _rankingService.GetUserRankingsWithDetailsAsync();
            var top = result.Data?.Take(count) ?? Enumerable.Empty<UserRankingDetailDto>();
            return Ok(new {
                Status = 200,
                Code = "SUCCESS",
                Data = top,
                Message = $"Top {count} user rankings fetched successfully."
            });
        }

        /// <summary>
        /// Get ranking details for a specific user (placeholder)
        /// </summary>
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserRanking(Guid userId)
        {
            // Placeholder: In the future, filter the ranking list for the specific user
            var result = await _rankingService.GetUserRankingsWithDetailsAsync();
            var userRanking = result.Data?.FirstOrDefault(r => r.UserId == userId);
            if (userRanking == null)
                return NotFound(new { Status = 404, Code = "NOT_FOUND", Message = "User ranking not found." });
            return Ok(new {
                Status = 200,
                Code = "SUCCESS",
                Data = userRanking,
                Message = "User ranking fetched successfully."
            });
        }
    }
} 