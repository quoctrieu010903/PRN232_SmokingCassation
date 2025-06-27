using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SmokingCessation.Application.DTOs.Request
{
    public class AchievementCreateRequest
    {
        public string Title { get; set; } = default!;
        public string? Description { get; set; }
        public IFormFile iconUrl { get; set; }
        public string ConditionType { get; set; }   // e.g., "DaysSmokeFree"
        public int ConditionValue { get; set; }

    }
}
