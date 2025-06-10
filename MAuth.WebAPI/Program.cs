using Mapster;
using MAuth.Data;
using Serilog;

using MAuth.WebAPI.Commons.Extensions;

using MAuth.WebAPI.Services.Identity;
using MAuth.WebAPI.Services.Players;
using MAuth.WebAPI.Services.Users;

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseConfiguredSerilog(builder.Environment);

    builder.Services.AddConfiguredPostgres(builder.Configuration, builder.Environment);

    // ���û���֧��
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration =
            builder.Configuration.GetConnectionString("RedisConnection");
    });

    // ������Դ����
    builder.Services.AddConfiguredCors();

    // API����������
    builder.Services.AddConfiguredApiController();

    // ����ӳ������
    builder.Services.AddMapster();

    // API�ĵ�����
    builder.Services.AddConfiguredSwagger(builder.Configuration);

    // JWT������֤����
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