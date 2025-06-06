using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmokingCessation.Domain.Enums;

namespace SmokingCessation.Application.DTOs.Request
{
    public class BlogRequest
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string? FeaturedImageUrl { get; set; }
        public BlogStatus Status { get; set; }


    }
}
