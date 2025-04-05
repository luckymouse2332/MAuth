using MAuth.Web.Models.Exceptions;
using MAuth.Web.Models.Responses;
using Serilog;

namespace MAuth.Web.Middlewares
{
    public class GlobalExceptionHandlerMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var statusCode = ex is CustomException customEx && customEx.StatusCode != 500
                    ? customEx.StatusCode
                    : 500;

                Log.Error("Exception occurred! Status: {0}, Message: {1}", statusCode, ex);

                context.Response.StatusCode = statusCode;
                await context.Response.WriteAsJsonAsync(
                    statusCode == 500 ? ResponseResult.Error(ex.Message) : ResponseResult.Fail(statusCode, ex.Message)
                );
            }
        }
    }

    public static class GlobalExceptionHandlerExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
        {
            return app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
        }
    }
}
