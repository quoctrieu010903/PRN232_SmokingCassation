
using System.ComponentModel.DataAnnotations;
using SmokingCessation.Core.Utils;


namespace SmokingCessation.Core.Base
{
    public abstract class BaseEntity
    {
        protected BaseEntity()
        {   
            CreatedTime = LastUpdatedTime = DateTime.UtcNow;
        }

        [Key]
        public Guid Id { get; set; }
        public string? CreatedBy { get; set; }
        public string? LastUpdatedBy { get; set; }
        public string? DeletedBy { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime LastUpdatedTime { get; set; }
        public DateTime? DeletedTime { get; set; }
    }
}
