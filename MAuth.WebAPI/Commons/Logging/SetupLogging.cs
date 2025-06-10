using Serilog;

namespace MAuth.WebAPI.Commons.Logging;

/// <summary>
/// 日志配置类
/// </summary>
public static class SetupLogging
{
    /// <summary>
    /// 开发模式下的日志配置
    /// </summary>
    /// <remarks>开发模式启用控制台日志，方便Debug</remarks>
    /// <returns>返回日志配置对象</returns>
    public static LoggerConfiguration Development()
    {
        var now = DateTime.Now;
        return new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Console()
            .WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LogFiles", $"{now.Year}-{now.Month}-{now.Day}", "Log.txt"),
                rollingInterval: RollingInterval.Infinite,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}] [{Level}] {Message}{NewLine}{Exception}");
    }

    /// <summary>
    /// 生产模式下的日志配置
    /// </summary>
    /// <remarks>为避免性能问题，生产模式不启用控制台日志</remarks>
    /// <returns>返回日志配置对象</returns>
    public static LoggerConfiguration Production()
    {
        return new LoggerConfiguration()
            .WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LogFiles", "Log.txt"),
                rollingInterval: RollingInterval.Day);
    }
}