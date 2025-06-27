using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmokingCessation.Application.DTOs.Response
{
    public class HealthImpactProgress
    {
        public double CancerRiskReductionPercent { get; set; }
        public double HeartRiskReductionPercent { get; set; }
        public string Summary { get; set; }
    }
}
