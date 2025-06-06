using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmokingCessation.Core.Base;
using SmokingCessation.Core.Utils;
using SmokingCessation.Domain.Enums;

namespace SmokingCessation.Domain.Entities
{
    public class Rating : BaseEntity
    {
        public Guid BlogId { get; set; }
        public Guid UserId { get; set; }
        public int Start { get; set; }
        public DateTimeOffset RatedAt { get; set; } = CoreHelper.SystemTimeNow;

       
        public Blog Blog { get; set; }
        public ApplicationUser User { get; set; }
    }
}
