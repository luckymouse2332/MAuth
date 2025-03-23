using MAuth.Web.Data;
using MAuth.Web.Data.Interceptors;
using MAuth.Web.Utils.Logging;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace MAuth.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        if (builder.Environment.IsDevelopment())
        {
            SetupLogging.Development();
            builder.Services.SensitiveDataLoggingConnection(builder);
        }
        else
        {
            SetupLogging.Production();
            builder.Services.ProductionLoggingConnection(builder);
        }

        builder.Host.UseSerilog();

        builder.Services.AddControllers();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddSingleton<SoftDeleteInterceptor>();

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;

        builder.Services.AddDbContext<MAuthDbContext>((serviceProvider, options) =>
        {
            options.UseNpgsql(connectionString)
                .AddInterceptors(
                    serviceProvider.GetRequiredService<SoftDeleteInterceptor>());
        });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}

