using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmokingCessation.Core.Base;

namespace SmokingCessation.Domain.Entities
{
    public class QuitPlanTask : BaseEntity
    {
        public Guid PlanTemplateId { get; set; }
        public int DayOffset { get; set; }
        public string Instruction { get; set; }

        public QuitPlanTemplate PlanTemplate { get; set; }
    }
}
