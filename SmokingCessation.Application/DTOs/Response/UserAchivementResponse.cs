using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmokingCessation.Domain.Entities;

namespace SmokingCessation.Application.DTOs.Response
{
    public class UserAchivementResponse
    {
        public string UserName { get; set; }
        public DateTime GrantedAt { get; set; }
        public List<AchievementResponse> Achievements { get; set; }

    }
}
