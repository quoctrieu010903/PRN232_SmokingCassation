
using SmokingCessation.Core.Base;
using SmokingCessation.Core.Utils;
using SmokingCessation.Domain.Enums;

namespace SmokingCessation.Domain.Entities
{
    public class Blog : BaseEntity
    {
        public Guid AuthorId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public BlogStatus Status { get; set; }
        public DateTime PublishedDate { get; set; } = DateTime.UtcNow;
        public string FeaturedImageUrl { get; set; }
        public int ViewCount { get; set; } = 0;

        // Navigation properties
        public ApplicationUser Author { get; set; }
        public ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();

      
    }
}
