using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using MAuth.Web.Data;

namespace MAuth.Web.Commons.Logging
{
    public static class DbContexts
    {
        /// <summary>
        /// 测试连接数据库是否可用
        /// </summary>
        /// <param name="context"><see cref="DbContext"/></param>
        /// <param name="ct">Provides a shorter time out from 30 seconds to in this case one second</param>
        /// <returns>true if database is accessible</returns>
        /// <remarks>
        /// Running asynchronous as synchronous.
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
        public static void SensitiveDataLoggingConnection(this IServiceCollection collection, WebApplicationBuilder builder)
        {

            collection.AddDbContextPool<MAuthDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection"))
                    .EnableSensitiveDataLogging()
                    .LogTo(new DbContextToFileLogger().Log));
        }

        /// <summary>
        /// 单行日志记录，启用 EF Core 的敏感数据
        /// </summary>
        public static void SingleLineSensitiveDataLoggingConnection(this IServiceCollection collection, WebApplicationBuilder builder)
        {

            collection.AddDbContextPool<MAuthDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection"))
                    .EnableSensitiveDataLogging().LogTo(
                        new DbContextToFileLogger().Log,
                        LogLevel.Debug,
                        DbContextLoggerOptions.DefaultWithLocalTime | DbContextLoggerOptions.SingleLine));

        }
        /// <summary>
        /// 生产环境下的日志记录
        /// </summary>
        /// <param name="collection"></param>
        public static void ProductionLoggingConnection(this IServiceCollection collection, WebApplicationBuilder builder)
        {

            collection.AddDbContextPool<MAuthDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection"))
                    .LogTo(new DbContextToFileLogger().Log));

        }
    }
}
