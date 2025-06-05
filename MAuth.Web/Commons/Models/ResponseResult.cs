using Microsoft.AspNetCore.Mvc;

namespace MAuth.Web.Commons.Models;

public class ResponseResult : ObjectResult
{
    public ResponseResult(int code, string message = "成功！", object? data = null)
        : base(new { Status = code, Message = message, Data = data })
    {
        StatusCode = code;
    }

    public static ResponseResult Success(string message = "成功！") =>
        new(StatusCodes.Status200OK, message);

    public static ResponseResult Success(object? data, string message = "成功！") =>
        new(StatusCodes.Status200OK, message, data);

    public static ResponseResult Fail(int code, string message = "失败！") =>
        new(code, message);

    public static ResponseResult Fail(object data, int code, string message = "失败！") =>
        new(code, message, data);

    public static ResponseResult Error(string message = "错误！") =>
        new(StatusCodes.Status500InternalServerError, message);
}
