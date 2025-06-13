using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmokingCessation.Domain.Enums;

namespace SmokingCessation.Application.DTOs.Request
{
    public class QuitPlansRequest
    {
        public string Reason { get; set; }
        
    }
}
