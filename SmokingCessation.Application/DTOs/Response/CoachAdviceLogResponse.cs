using System;

namespace SmokingCessation.Application.DTOs.Response
{
    public class CoachAdviceLogResponse
    {
        public Guid Id { get; set; }
        public string QuitPlanReason { get; set; }
        public string UserName { get; set; }
        public DateTimeOffset AdviceDate { get; set; }
        public string AdviceText { get; set; }
    }
} 