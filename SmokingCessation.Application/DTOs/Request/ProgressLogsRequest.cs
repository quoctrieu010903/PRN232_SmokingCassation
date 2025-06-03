using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmokingCessation.Application.DTOs.Request
{
    public class ProgressLogsRequest
    {
        public Guid QuitPlanId { get; set; }
        public int SmokedToday { get; set; }
        public string Note { get; set; }
    }
}
