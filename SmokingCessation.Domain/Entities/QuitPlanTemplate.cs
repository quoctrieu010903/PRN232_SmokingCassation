using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmokingCessation.Core.Base;

namespace SmokingCessation.Domain.Entities
{
    public class QuitPlanTemplate : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int DaysToTarget { get; set; }
        public Guid PackageId { get; set; }
        public MembershipPackage Package { get; set; }
        public ICollection<QuitPlanTask> TaskTemplates { get; set; }
        public ICollection<QuitPlan> QuitPlans { get; set; }
    }
}
