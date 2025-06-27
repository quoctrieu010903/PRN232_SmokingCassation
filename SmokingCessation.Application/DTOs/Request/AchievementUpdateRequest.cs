using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SmokingCessation.Application.DTOs.Request
{
    public class AchievementUpdateRequest
    {
        public string Title { get; set; } = default!;
        public string? Description { get; set; }
    }
}
