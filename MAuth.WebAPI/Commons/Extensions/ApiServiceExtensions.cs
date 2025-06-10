using MAuth.WebAPI.Commons.Filters;
using MAuth.WebAPI.Commons.Models;
using Microsoft.AspNetCore.Mvc;

namespace MAuth.WebAPI.Commons.Extensions;

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
        services.AddControllers(options =>
        {
            options.Filters.Add(new ResultFilter());
        }).ConfigureApiBehaviorOptions(options =>
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
    private static UnprocessableEntityObjectResult ProcessInvalidModelState(ActionContext context)
    {
        var problemDetails = new ValidationProblemDetails(context.ModelState)
        {
            Status = StatusCodes.Status422UnprocessableEntity,
            Instance = context.HttpContext.Request.Path,
            Title = "无法处理的请求数据"
        };

        problemDetails.Extensions.Add("traceId", context.HttpContext.TraceIdentifier);
        
        return new UnprocessableEntityObjectResult(ResponseResult.Fail("无法处理的请求数据！", problemDetails))
        {
            ContentTypes = ["application/problem+json"]
        };
    }
}
