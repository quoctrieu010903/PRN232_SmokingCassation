using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmokingCessation.Domain.Enums;

namespace SmokingCessation.Application.DTOs.Request
{
    public class QuitPlansRequest
    {
        public string Reason { get; set; }
        public int CigarettesPerDayBeforeQuit { get; set; }   // ✅ Bao nhiêu điếu mỗi ngày
        public int YearsSmokingBeforeQuit { get; set; } // đã hút được trong vòng bao lâu trước khi quit

    }
}
