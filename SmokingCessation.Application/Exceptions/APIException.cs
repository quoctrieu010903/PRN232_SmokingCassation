

namespace SmokingCessation.Application.Exceptions
{
    public class APIException : Exception
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        public APIException(int StatusCode, string Message, object Data = null)
        {
            this.StatusCode = StatusCode;
            this.Message = Message;
            this.Data = Data;
        }
    }
}
