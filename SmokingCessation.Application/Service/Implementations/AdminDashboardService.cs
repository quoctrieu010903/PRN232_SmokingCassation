
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmokingCessation.Application.DTOs.Request;
using SmokingCessation.Application.DTOs.Response;
using SmokingCessation.Application.Service.Interface;
using SmokingCessation.Core.Constants;
using SmokingCessation.Core.Response;
using SmokingCessation.Domain.Entities;
using SmokingCessation.Domain.Enums;
using SmokingCessation.Domain.Interfaces;

namespace SmokingCessation.Application.Service.Implementations
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBlogService _blogService;
        private readonly IFeedbackService _feedbackService;

        public AdminDashboardService(IUnitOfWork unitOfWork, IBlogService blogService, IFeedbackService feedbackService)
        {
            _unitOfWork = unitOfWork;
            _blogService = blogService;
            _feedbackService = feedbackService;
        }

        public async Task<BaseResponseModel<AdminDashboardSummaryDto>> GetSummaryAsync()
        {
            var users = await _unitOfWork.Repository<ApplicationUser, ApplicationUser>().GetAllAsync();
            var blogs = await _unitOfWork.Repository<Blog, Blog>().GetAllAsync();
            var ratings = await _unitOfWork.Repository<Rating, Rating>().GetAllAsync();
            var quitPlans = await _unitOfWork.Repository<QuitPlan, QuitPlan>().GetAllAsync();
            var payments = await _unitOfWork.Repository<Payment, Payment>().GetAllAsync();
            var feedbacks = await _unitOfWork.Repository<Feedback, Feedback>().GetAllAsync();

            var summary = new AdminDashboardSummaryDto
            {
                TotalUsers = users.Count(),
                NewUsersThisMonth = users.Count(u => u.CreatedTime.Month == DateTime.UtcNow.Month && u.CreatedTime.Year == DateTime.UtcNow.Year),
                TotalBlogs = blogs.Count(),
                PublishedBlogs = blogs.Count(b => b.Status == BlogStatus.Published),
                PendingBlogs = blogs.Count(b => b.Status == BlogStatus.Pending_Approval),
                TotalRatings = ratings.Count(),
                AverageRating = ratings.Any() ? ratings.Average(r => r.Start) : 0,
                TotalQuitPlans = quitPlans.Count(),
                ActiveQuitPlans = quitPlans.Count(q => q.Status == QuitPlanStatus.Active),
                CompletedQuitPlans = quitPlans.Count(q => q.Status == QuitPlanStatus.Completed),
                TotalRevenue = payments.Where(p => p.Status == PaymentStatus.Success).Sum(p => p.Amount),


            };

            return new BaseResponseModel<AdminDashboardSummaryDto>(
                200,
                ResponseCodeConstants.SUCCESS,
                summary,
                "Admin dashboard summary fetched successfully."
            );
        }

        
    }

     
}
