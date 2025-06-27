using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmokingCessation.Application.DTOs.Response
{
    public class AchievementResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string iconUrl { get; set; }
        public string ConditionType { get; set; }   // e.g., "DaysSmokeFree"
        public int ConditionValue { get; set; }

    }
}
