using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmokingCessation.Application.DTOs.Fillter;
using SmokingCessation.Application.DTOs.Request;
using SmokingCessation.Application.Service.Interface;
using SmokingCessation.Core.Constants;

namespace SmokingCessation.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminManagerController : ControllerBase
    {
        private readonly IBlogService _blogService;
        private readonly IFeedbackService _feedbackService;
        private readonly IQuitPlanService _quitPlanService;
        private readonly IAchivementServie _achivementServie;
        private readonly IUserPackageService _userPackageService;
        private readonly IUserAchievementService _userAchievementService;
        private readonly IMemberShipPackage _memberShipPackageService;
        private readonly IAuthenticationService _authenticationService;
        private readonly ICoachAdviceLogService _coachAdviceLogService;



        public AdminManagerController(IBlogService blogService, IFeedbackService feedbackService, IQuitPlanService quitPlanService, IAchivementServie achivementServie , IUserAchievementService userAchievementService, IAuthenticationService authenticationService , IMemberShipPackage memberShipPackage , ICoachAdviceLogService coachAdviceLogService)
        {
            _blogService = blogService;
            _feedbackService = feedbackService;
            _quitPlanService = quitPlanService;
            _achivementServie = achivementServie;
            _memberShipPackageService = memberShipPackage;
            _coachAdviceLogService = coachAdviceLogService;

            _userAchievementService = userAchievementService;
            _authenticationService = authenticationService;
        }
        [HttpGet("Blogs")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> GetBlogs([FromQuery] PagingRequestModel paging, BlogListFilter fillter)
        {
            var result = await _blogService.GetAll(paging,fillter);
            return Ok(result);
        }
        [HttpGet("QuitPlans")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> GetQuitPlans([FromQuery] PagingRequestModel paging, QuitPlanFillter fillter)
        {
            var result = await _quitPlanService.getAllQuitPlan(paging,fillter , false);
            return Ok(result);
        }
        [HttpGet("Achivements")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> GetAchivements([FromQuery] PagingRequestModel paging)
        {
            var result = await _achivementServie.GetAllAsync(paging);
            return Ok(result);
        }
        [HttpGet("Users")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> GetUsers([FromQuery] PagingRequestModel paging)
        {
            var result = await _authenticationService.GetAllUser(paging);
            return Ok(result);
        }
       
     
        [HttpGet("UserAchievements")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> GetUserAchievements([FromQuery] PagingRequestModel paging)
        {
            var result = await _userAchievementService.getAllUserAchivement(paging);
            return Ok(result);
        }
      
      
        [HttpGet("MemberShipPackages")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> GetMemberShipPackages([FromQuery] PagingRequestModel paging, MemberShipPackageFillter fillter)
        {
            var result = await _memberShipPackageService.getAllPackage(paging,fillter);
            return Ok(result);
        }

       
    }
}
