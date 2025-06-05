using Mapster;
using Serilog;

using MAuth.Web.Commons.Extensions;

using MAuth.Web.Services.Identity;
using MAuth.Web.Services.Players;
using MAuth.Web.Services.Users;
using MAuth.Web.Data;

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseConfiguredSerilog(builder.Environment);

    builder.Services.AddConfiguredPostgres(builder.Configuration, builder.Environment);

    // 配置缓存支持
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration =
            builder.Configuration.GetConnectionString("RedisConnection");
    });

    // 跨域资源共享
    builder.Services.AddConfiguredCors();

    // API控制器配置
    builder.Services.AddConfiguredApiController();

    // 对象映射配置
    builder.Services.AddMapster();

    // API文档配置
    builder.Services.AddConfiguredSwagger(builder.Configuration);

    // JWT身份认证配置
    builder.Services.ConfigureJwtOptions(builder.Configuration)
        .ConfigureJwtSigningCredentials()
        .AddConfiguredAuthentication()
        .AddConfiguredAuthorization();
    
    builder.Services.AddScoped<IUserRepository, UserRepository>();

    builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();

    builder.Services.AddSingleton<ITokenStore, RefreshTokenRedisStore>();

    builder.Services.AddScoped<IIdentityService, IdentityService>();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseCors();

    app.UseResponseCaching();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application startup failed");
    throw;
}
finally
{
    Log.CloseAndFlush();
}