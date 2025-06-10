using MAuth.Web.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MAuth.Data;

/// <summary>
/// 数据库上下文配置和日志记录相关的扩展方法
/// </summary>
public static class DbContexts
{
    /// <summary>
    /// 注入配置好的数据库连接
    /// </summary>
    /// <param name="services"></param>
    /// <param name="config"></param>
    /// <param name="env"></param>
    /// <returns></returns>
    public static IServiceCollection AddConfiguredPostgres(this IServiceCollection services, IConfiguration config, IHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            services.SensitiveDataLoggingConnection(config);
        }
        else
        {
            services.ProductionLoggingConnection(config);
        }

        return services;
    }

    /// <summary>
    /// 测试连接数据库是否可用
    /// </summary>
    /// <param name="context"><see cref="DbContext"/></param>
    /// <param name="ct">Provides a shorter time out from 30 seconds to in this case one second</param>
    /// <returns>如果数据库可连接则返回true</returns>
    /// <remarks>
    /// 此方法以同步运行的方式运行了异步方法
    /// </remarks>
    public static bool CanConnectAsync(this DbContext context, CancellationToken ct)
    {
        try
        {
            return context.Database.CanConnectAsync(ct).Result;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 为 EF Core 启用敏感数据日志记录
    /// </summary>
    public static void SensitiveDataLoggingConnection(this IServiceCollection collection, IConfiguration config)
    {

        collection.AddDbContextPool<MAuthDbContext>(options =>
            options.UseNpgsql(config.GetConnectionString("PostgresConnection"))
                .EnableSensitiveDataLogging()
                .LogTo(new DbContextToFileLogger().Log));
    }

    /// <summary>
    /// 单行日志记录，启用 EF Core 的敏感数据
    /// </summary>
    public static void SingleLineSensitiveDataLoggingConnection(this IServiceCollection collection, IConfiguration config)
    {

        collection.AddDbContextPool<MAuthDbContext>(options =>
            options.UseNpgsql(config.GetConnectionString("PostgresConnection"))
                .EnableSensitiveDataLogging().LogTo(
                    new DbContextToFileLogger().Log,
                    LogLevel.Debug,
                    DbContextLoggerOptions.DefaultWithLocalTime | DbContextLoggerOptions.SingleLine));
    }
    /// <summary>
    /// 生产环境下的日志记录
    /// </summary>
    public static void ProductionLoggingConnection(this IServiceCollection collection, IConfiguration config)
    {

        collection.AddDbContextPool<MAuthDbContext>(options =>
            options.UseNpgsql(config.GetConnectionString("PostgresConnection"))
                .LogTo(new DbContextToFileLogger().Log));
    }
}