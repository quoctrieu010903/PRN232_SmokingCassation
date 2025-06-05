using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmokingCessation.Core.Base;

namespace SmokingCessation.Domain.Entities
{
    public class Feedback :BaseEntity
    {
        public Guid BlogId { get; set; }
        public Guid UserId { get; set; }
        public string Comment { get; set; }
        public bool IsApproved { get; set; }  = false;
        public Blog Blog { get; set; }
        public ApplicationUser User { get; set; }
    }
}
