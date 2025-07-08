using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmokingCessation.Application.DTOs.Response
{
    public class UserDashboardDto
    {
        public int CigarettesThisWeek { get; set; }
        public int CigarettesThisMonth { get; set; }
        public int TotalSmokeFreeDays { get; set; }

        // For week view: each day's count in the selected week
        public List<DailyCigaretteCountDto> DailyCountsThisWeek { get; set; }

        // For month view: each day's count in the selected month
        public List<DailyCigaretteCountDto> DailyCountsThisMonth { get; set; }


    }
}
