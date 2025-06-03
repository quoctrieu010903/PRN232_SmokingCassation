using SmokingCessation.Core.Base;

namespace SmokingCessation.Domain.Entities
{
    public class ProgressLog : BaseEntity
    {
        public Guid QuitPlanId { get; set; }
        public DateTimeOffset LogDate { get; set; }
        public int SmokedToday { get; set; }
        public string Note { get; set; }

        public QuitPlan QuitPlan { get; set; }

    }
}