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
        public string reason { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime TargetDate { get; set; }
        public int CreateNum { get; set; }
        public QuitPlanStatus status { get; set; }

      
        public Guid UserId { get; set; }

    
        public virtual ApplicationUser User { get; set; }
        public ICollection<ProgressLog> ProgressLogs { get; set; }
    }
}
