using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmokingCessation.Core.Constants;

namespace SmokingCessation.Application.DTOs.Request
{
    public class PagingRequestModel
    {

        [Range(1, int.MaxValue, ErrorMessage = "Số trang phải lớn hơn 1")]
        public int PageNumber { get; set; } = 1;

        [Range(1, int.MaxValue, ErrorMessage = "Sức chứa 1 trang phải lớn hơn 1")]
        public int PageSize { get; set; } = 20;

    }
}
