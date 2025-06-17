using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmokingCessation.Domain.Enums;

namespace SmokingCessation.Application.DTOs.Fillter
{
    public class BlogListFilter
    {
        public BlogListFilterType FilterType { get; set; } = BlogListFilterType.All;
        public string? Search { get; set; }
        public BlogStatus? Status { get; set; } // Chỉ FE admin truyền lên

    }
}
