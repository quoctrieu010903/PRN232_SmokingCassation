using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmokingCessation.Domain.Enums;

namespace SmokingCessation.Application.DTOs.Response
{
    public class QuitPlanResponse
    {
        public Guid Id { get; set; }
        public string Reason { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset TargetDate { get; set; }
        public int CigarettesPerDayBeforeQuit { get; set; }   // ✅ Bao nhiêu điếu mỗi ngày
        public int YearsSmokingBeforeQuit { get; set; }       // 
        public string Status { get; set; }
        public Guid UserId { get; set; }
        public Guid PackageId { get; set; }
        public string PackageName { get; set; }
        public string AdviceText { get; set; }
        public int SmokeFreeDays { get; set; }
        public HealthImpactProgress HealthImpact { get; set; }
    }
}
