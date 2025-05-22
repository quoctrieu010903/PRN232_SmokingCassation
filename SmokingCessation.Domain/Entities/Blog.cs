
using SmokingCessation.Core.Base;
using SmokingCessation.Domain.Enums;

namespace SmokingCessation.Domain.Entities
{
    public class Blog : BaseEntity
    {
        public Guid AuthorId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public BlogStatus  Status   { get; set; }
        public ApplicationUser Author { get; set; }

    }
}
