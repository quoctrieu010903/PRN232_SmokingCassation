using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmokingCessation.Application.DTOs.Response
{
    public class RatingResponse
    {
        public Guid Id { get; set; }
        public Guid BlogId { get; set; }
        public string BlogTitle { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public int Value { get; set; }
        public string? Comment { get; set; }
        public DateTimeOffset RatedAt { get; set; }

    }
}
