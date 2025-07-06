using SmokingCessation.Core.Base;

namespace SmokingCessation.Domain.Entities
{
    public class ProgressLog : BaseEntity
    {
        public Guid QuitPlanId { get; set; }
        public DateTime LogDate { get; set; }
        public int SmokedToday { get; set; }
        public string Note { get; set; }    
        public QuitPlan QuitPlan { get; set; }

    }
}