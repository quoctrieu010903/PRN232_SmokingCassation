using System;

namespace SmokingCessation.Application.DTOs.Response
{
    public class AdminDashboardSummaryDto
    {
        public int TotalUsers { get; set; }
        public int NewUsersThisMonth { get; set; }
        public int TotalBlogs { get; set; }
        public int PublishedBlogs { get; set; }
        public int PendingBlogs { get; set; }
        public int TotalRatings { get; set; }
        public double AverageRating { get; set; }
        public int TotalQuitPlans { get; set; }
        public int ActiveQuitPlans { get; set; }
        public int CompletedQuitPlans { get; set; }
        public decimal TotalRevenue { get; set; }
    }
} 