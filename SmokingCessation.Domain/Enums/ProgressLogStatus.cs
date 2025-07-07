using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmokingCessation.Domain.Enums
{
    public enum ProgressLogStatus
    {
        Pending = 0,   // Mới được hệ thống tạo, chờ người dùng cập nhật
        Completed = 1, // Người dùng đã cập nhật là hoàn thành mục tiêu ngày
        Failed = 2     // Người dùng đã cập nhật là không hoàn thành mục tiêu ngày
    }
}
