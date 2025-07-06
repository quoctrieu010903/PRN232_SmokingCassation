using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmokingCessation.Domain.Enums;

namespace SmokingCessation.Application.DTOs.Fillter
{
    public class QuitPlanFillter
    {
        /// <summary>
        /// Ngày bắt đầu kế hoạch bỏ thuốc
        /// </summary>
        public string? StartDate { get; set; }

        /// <summary>
        /// Ngày mục tiêu kết thúc kế hoạch bỏ thuốc
        /// </summary>
        public string? TargetDate { get; set; }

        /// <summary>
        /// Trạng thái hiện tại của kế hoạch
        /// 0 is IsInactime, 1 is Active, 2 is Completed
        /// </summary>
        public QuitPlanStatus Status { get; set; }

        /// <summary>
        /// Tên người dùng cần tìm
        /// </summary>
        public string? UserName { get; set; }
    }
}
