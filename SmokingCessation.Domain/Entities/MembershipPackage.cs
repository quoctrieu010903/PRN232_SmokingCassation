using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmokingCessation.Domain.Entities
{
    public class MembershipPackage
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public decimal Price { get; set; }
       public MembershipType Type { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Payment> Payments { get; set; }
    }
}
