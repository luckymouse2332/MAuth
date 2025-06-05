using MAuth.Web.Commons.Models;
using Microsoft.AspNetCore.Mvc;

namespace MAuth.Web.Commons.Extensions;

/// <summary>
/// API控制器配置扩展方法
/// </summary>
public static class ApiServiceExtensions
{
    /// <summary>
    /// 配置API控制器服务
    /// </summary>
    public static IServiceCollection AddConfiguredApiController(this IServiceCollection services)
    {
        services.AddControllers().ConfigureApiBehaviorOptions(options =>
        {
            options.InvalidModelStateResponseFactory = ProcessInvalidModelState;
        });
        return services;
    }

    /// <summary>
    /// 配置跨域资源共享（CORS）服务
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddConfiguredCors(this IServiceCollection services)
    {
        return services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
        });
    }

    /// <summary>
    /// 处理失败的模型状态验证
    /// </summary>
    /// <param name="context"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    private static ResponseResult ProcessInvalidModelState(ActionContext context)
    {
        var problemDetails = new ValidationProblemDetails(context.ModelState)
        {
            Status = StatusCodes.Status422UnprocessableEntity,
            Instance = context.HttpContext.Request.Path,
            Title = "Unprocessable Entity",
        };

        problemDetails.Extensions.Add("traceId", context.HttpContext.TraceIdentifier);
        
        return new ResponseResult(StatusCodes.Status422UnprocessableEntity, "请求处理失败！", problemDetails)
        {
            ContentTypes = ["application/problem+json"],
        };
    }
}
