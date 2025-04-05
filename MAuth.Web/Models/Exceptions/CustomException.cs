using MAuth.Web.Models.Responses;

namespace MAuth.Web.Models.Exceptions
{
    /// <summary>
    /// 自定义异常类
    /// </summary>
    public class CustomException : Exception
    {
        public CustomException(ResponseStatus status) : base(status.Message)
        {
            StatusCode = status.Code;
        }

        public CustomException(int code, string message) : base(message)
        {
            StatusCode = code;
        }

        public int StatusCode { get; set; }

        public override string ToString()
        {
#if DEBUG
            return base.ToString();
#else
            return Message;
#endif
        }
    }
}
