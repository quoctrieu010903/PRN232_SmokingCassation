
using SmokingCessation.Core.Base;

namespace SmokingCessation.Domain.Entities
{
    public class CoachAdviceLog : BaseEntity
    {
        public Guid QuitPlanId { get; set; }                    // Kế hoạch liên quan
        public DateTimeOffset AdviceDate { get; set; }          // Ngày hướng dẫn
        public string AdviceText { get; set; }                  // Nội dung được AI sinh ra

        public QuitPlan QuitPlan { get; set; }
    }
}
