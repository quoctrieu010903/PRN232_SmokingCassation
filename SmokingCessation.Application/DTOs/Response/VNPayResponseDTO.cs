using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmokingCessation.Application.DTOs.Response
{
    public class VNPayResponseDTO
    {
        public string vnp_TmnCode { get; set; }
        public string vnp_TxnRef { get; set; }
        public float vnp_Amount { get; set; }
        public string vnp_OrderInfo { get; set; }
        public string vnp_BankCode { get; set; }
        public string? vnp_BankTranNo { get; set; }
        public string? vnp_CardType { get; set; }
        public string vnp_PayDate { get; set; }
        public string vnp_TransactionNo { get; set; }
        public bool isSuccess { get; set; }
    }
}
