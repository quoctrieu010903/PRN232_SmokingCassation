    using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmokingCessation.Core.Base;
using SmokingCessation.Domain.Enums;

namespace SmokingCessation.Domain.Entities
{
    public class Payment : BaseEntity
    {

        public decimal Amount { get; set; }

        public PaymentStatus Status { get; set; }
    

        public Guid PackageId { get; set; }
        public Guid UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual MembershipPackage Package { get; set; }
      
    }
}
