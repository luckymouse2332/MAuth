using Serilog;

namespace MAuth.Web.Helpers.Logging;

public static class SetupLogging
{
    public static void Development()
    {
        var now = DateTime.Now;

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Console()
            .WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LogFiles", $"{now.Year}-{now.Month}-{now.Day}", "Log.txt"),
                rollingInterval: RollingInterval.Infinite,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level}] {Message}{NewLine}{Exception}")
            .CreateLogger();
    }

    public static void Production()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LogFiles", "Log.txt"),
                rollingInterval: RollingInterval.Day)
            .CreateBootstrapLogger();
    }
}
