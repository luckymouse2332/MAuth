using MAuth.WebAPI.Commons.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MAuth.WebAPI.Commons.Filters;

/// <summary>
/// 仅处理成功请求
/// </summary>
public class ResultFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Result is not ObjectResult result)
        {
            return;
        }

        context.Result = context.HttpContext.Response.StatusCode switch
        {
            >= 200 and <= 299 => CreateResult(ResponseStatus.Ok, result.Value),
            >= 400 and < 500 => CreateResult(ResponseStatus.Fail, result.Value),
            >= 500 => CreateResult(ResponseStatus.Error, result.Value),
            _ => CreateResult(ResponseStatus.Ok, result.Value)
        };
    }

    private static ObjectResult CreateResult(ResponseStatus status, object? value)
    {
        return value switch
        {
            null => new ObjectResult(ResponseResult.Result(status, null, null)),
            string str => new ObjectResult(ResponseResult.Result(status, str, null)),
            _ => new ObjectResult(ResponseResult.Result(status, null, value))
        };
    }
}