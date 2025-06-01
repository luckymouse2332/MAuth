namespace MAuth.Web.Commons.Models
{
    [Serializable]
    public class ResponseResult<TData>(int code, string message, TData data) : ResponseResult(code, message)
    {
        public TData? Data { get; set; } = data;
    }

    [Serializable]
    public class ResponseResult(int code, string message)
    {
        public int Status { get; set; } = code;

        public string Message { get; set; } = message;

        public static ResponseResult Success(string message = "成功！") =>
            new(200, message);

        public static ResponseResult<T> Success<T>(T data, string message = "成功！") =>
            new(200, message, data);

        public static ResponseResult Fail(int code, string message = "失败！") =>
            new(code, message);

        public static ResponseResult<T> Fail<T>(T data, int code, string message = "失败！") =>
            new(code, message, data);

        public static ResponseResult Error(string message = "错误！") =>
            new(500, message);
    }
}
