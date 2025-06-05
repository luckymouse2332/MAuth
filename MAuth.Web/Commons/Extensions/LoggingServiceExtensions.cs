using MAuth.Web.Commons.Logging;
using Serilog;

namespace MAuth.Web.Commons.Extensions;

/// <summary>
/// 配置日志记录服务的扩展方法
/// </summary>
public static class LoggingServiceExtensions
{
    public static IHostBuilder UseConfiguredSerilog(this IHostBuilder hostBuilder, IHostEnvironment env)
    {
        var loggerConfig = env.IsDevelopment()
            ? SetupLogging.Development()
            : SetupLogging.Production();

        // 设置静态全局 Log.Logger，同时供系统日志使用
        Log.Logger = loggerConfig.CreateLogger();

        return hostBuilder.UseSerilog(Log.Logger);
    }
}
