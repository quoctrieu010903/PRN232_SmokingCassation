    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmokingCessation.Application.DTOs.Request
{
    public class FeedbackRequest
    {
        public Guid BlogId { get; set; }
        public string Comment { get; set; }
    }
}
