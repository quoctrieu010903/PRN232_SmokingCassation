using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmokingCessation.Application.DTOs.Response
{
    public class UserRankingDetailDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; }
        public int AchievementCount { get; set; }
        public int TotalSmokeFreeDays { get; set; }
        public int Rank { get; set; }
        public decimal TotalMoneySaved { get; set; } // Số tiền tiết kiệm được
        public List<BadgeDto> Badges { get; set; } = new();
        public string RelapseRisk { get; set; } = "Dữ liệu không đủ"; // Tỉ lệ/rủi ro mắc bệnh lại (ví dụ)


    }
}
