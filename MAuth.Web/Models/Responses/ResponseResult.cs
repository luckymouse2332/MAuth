namespace MAuth.Web.Models.Responses
{
    [Serializable]
    public class ResponseResult<TData> : ResponseResult
    {
        public ResponseResult(int code, string message, TData data) : base(code, message)
        {
            Data = data;
        }

        public ResponseResult(ResponseStatus status, TData data) : base(status)
        {
            Data = data;
        }

        public TData? Data { get; set; }
    }

    [Serializable]
    public class ResponseResult
    {
        public ResponseResult(int code, string message)
        {
            Status = code;
            Message = message;
        }

        public ResponseResult(ResponseStatus status)
        {
            Status = status.Code;
            Message = status.Message;
        }

        public int Status { get; set; }

        public string Message { get; set; }

        public static ResponseResult Success(string message = "成功！") =>
            new(200, message);

        public static ResponseResult<T> Success<T>(T data, string message = "成功！") =>
            new(200, message, data);

        public static ResponseResult Fail(int code, string message = "失败！") =>
            new(code, message);

        public static ResponseResult Fail(ResponseStatus status) =>
            new(status);

        public static ResponseResult Error(string message = "错误！") =>
            new(500, message);
    }
}
