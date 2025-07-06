using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmokingCessation.Application.DTOs.Response
{
    public class ProgressLogsResponse
    {
        public Guid Id { get; set; }
        public string  QuitPlanName { get; set; }
        public DateTimeOffset  LogDate { get; set; }
        public int SmokedToday { get; set; }
        public string Note { get; set; }
        public string CoachAdvice { get; set; }
    }
}
