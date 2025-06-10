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
        public string Reason { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset TargetDate { get; set; }
        public QuitPlanStatus Status { get; set; }

        public Guid UserId { get; set; }
        public Guid TemplateId { get; set; }

    
     
        public ApplicationUser User { get; set; }
        public QuitPlanTemplate Template { get; set; }
        public ICollection<ProgressLog> ProgressLogs { get; set; }


      
    }
}
