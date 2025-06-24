using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmokingCessation.Application.DTOs.Response
{
    public class BlogResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string? FeaturedImageUrl { get; set; }
        public int Views {  get; set; }
        public string Status { get; set; }
        public DateTimeOffset PublishedDate { get; set; }
        public string AuthorName { get; set; }
        public double AverageRating { get; set; } // Chỉ là property, không lưu DB
        public List<FeedbackResponse> Comments { get; set; } = new();


    }
}
