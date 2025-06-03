using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmokingCessation.Application.DTOs.Fillter
{
    public class ProgressLogsFillter
    {
        public string? QuitPlanName { get; set; }
        public DateTimeOffset LogDate { get; set; }
        public string? Note { get; set; }
    }
}
