using MAuth.Web.Data;
using MAuth.Web.Data.Extensions;
using MAuth.Web.Mappings;
using MAuth.Web.Utils.Logging;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace MAuth.Web
{
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

            builder.Services.AddDbContextFactory<MAuthDbContext>(options =>
            {
                var connectionString = 
                    builder.Configuration.GetConnectionString("DefaultConnection");
                options.UseNpgsql(connectionString);
            });


            builder.Services.AddRepositories();

            builder.Services.AddAutoMapper(typeof(UserMapperProfile).Assembly);

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseRouting();

            app.MapControllers();

            app.Run();
        }
    }
}
