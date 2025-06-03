using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmokingCessation.Domain.Enums;

namespace SmokingCessation.Application.DTOs.Fillter
{
    public class QuitPlanFillter
    {
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset TargetDate { get; set; }
        public QuitPlanStatus Status { get; set; }
        public String? UserName { get; set; }
    }
}
