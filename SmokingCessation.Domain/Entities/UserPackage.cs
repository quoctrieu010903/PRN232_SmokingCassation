using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmokingCessation.Core.Base;

namespace SmokingCessation.Domain.Entities
{
    public class UserPackage : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid PackageId { get; set; }

        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public bool IsActive { get; set; }

        public DateTimeOffset? CancelledDate { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual MembershipPackage Package { get; set; }
    }
}
