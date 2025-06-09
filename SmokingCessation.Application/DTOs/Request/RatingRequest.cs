using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmokingCessation.Application.DTOs.Request
{
    public class RatingRequest
    {
        public Guid BlogId { get; set; }
        public int Value { get; set; } // 1-5 stars

    }
}
