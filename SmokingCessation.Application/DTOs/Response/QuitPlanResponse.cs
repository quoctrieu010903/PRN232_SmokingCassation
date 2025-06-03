using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmokingCessation.Domain.Enums;

namespace SmokingCessation.Application.DTOs.Response
{
    public class QuitPlanResponse
    {
        public Guid Id { get; set; }
        public string Reason { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset TargetDate { get; set; }
        public int CreateNum { get; set; } // số lần user tạo một plan mới 
        public QuitPlanStatus Status { get; set; }
        public Guid UserId { get; set; }
        public Guid PackageId { get; set; }
    }
}
