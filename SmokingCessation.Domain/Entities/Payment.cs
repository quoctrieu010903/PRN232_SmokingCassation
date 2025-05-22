using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmokingCessation.Domain.Entities
{
    public class Payment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public decimal Amount { get; set; }

        [Column(TypeName = "varchar(24)")]
        public PaymentStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }

        // Foreign keys
        public Guid UserId { get; set; }
        public Guid PackageId { get; set; }

        // Navigation properties
        public virtual ApplicationUser User { get; set; }
        public virtual MembershipPackage Package { get; set; }
    }
}
