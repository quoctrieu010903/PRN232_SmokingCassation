using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmokingCessation.Core.Base;
using SmokingCessation.Domain.Enums;

namespace SmokingCessation.Domain.Entities
{
    public class QuitPlan : BaseEntity
    {
        public string Reason { get; set; }                      // Lý do bỏ thuốc
        public DateTime StartDate { get; set; }           // Ngày bắt đầu
        public DateTime TargetDate { get; set; }          // Ngày mục tiêu
        public QuitPlanStatus Status { get; set; }              // Trạng thái (Đang tiến hành, Hoàn thành, Huỷ)

        public int CigarettesPerDayBeforeQuit { get; set; }   // ✅ Bao nhiêu điếu mỗi ngày
        public int YearsSmokingBeforeQuit { get; set; }       // ✅ Bao nhiêu năm đã hút

        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }

        // ✅ Optional: nếu muốn biết kế hoạch thuộc gói nào
        public Guid? MembershipPackageId { get; set; } // Có thể nullable
        public MembershipPackage MembershipPackage { get; set; }
        public ICollection<ProgressLog> ProgressLogs { get; set; }
        public ICollection<CoachAdviceLog> AdviceLogs { get; set; }



    }
}
