using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmokingCessation.Core.Base;

namespace SmokingCessation.Domain.Entities
{
    public class Ranking :BaseEntity
    {
        public Guid UserId { get; set; }
        public int TotalSmokeFreeDays { get; set; }
        public int AchievementCount { get; set; }
        public ApplicationUser User { get; set; }
    }
}
