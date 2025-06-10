using System.ComponentModel;
using MAuth.WebAPI.Commons.Helpers;

namespace MAuth.WebAPI.Commons.Models;

/// <summary>
/// 统一响应封装
/// </summary>
public class ResponseResult
{
    public ResponseStatus Status { get; set; }

    private string? _message;

    public string Message
    {
        get => _message ?? EnumHelper.GetDescription(Status)!;
        set => _message = value;
    }

    public object? Data { get; set; }

    public static ResponseResult Ok() => Ok(null, null);

    public static ResponseResult Ok(string? message) => Ok(message, null);

    public static ResponseResult Ok(string? message, object? data) => new ResponseResult
        { Status = ResponseStatus.Ok, Data = data, Message = message };

    public static ResponseResult Fail() => Fail(null);

    public static ResponseResult Fail(string? message) => Fail(null, message);

    public static ResponseResult Fail(string? message, object? data) => new ResponseResult
        { Status = ResponseStatus.Fail, Data = data, Message = message };

    public static ResponseResult Error(string message) =>
        new ResponseResult { Status = ResponseStatus.Error, Message = message };

    public static ResponseResult Result(ResponseStatus status, string? message, object? data) =>
        new ResponseResult { Status = status, Message = message, Data = data };
}

/// <summary>
/// 响应状态
/// </summary>
public enum ResponseStatus
{
    [Description("成功")] Ok = 0,

    [Description("失败")] Fail = 1,

    [Description("错误")] Error = -1
}