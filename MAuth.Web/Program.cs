using Mapster;
using MAuth.Contracts.Enums;
using MAuth.Web.Commons.Filters;
using MAuth.Web.Commons.Helpers;
using MAuth.Web.Commons.Logging;
using MAuth.Web.Data;
using MAuth.Web.Services.Identity;
using MAuth.Web.Services.Players;
using MAuth.Web.Services.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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

// ������Դ����
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// API����������
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ResponseWrapperFilter>();
}).ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var problemDetails = new ValidationProblemDetails(context.ModelState)
        {
            Status = StatusCodes.Status422UnprocessableEntity,
            Instance = context.HttpContext.Request.Path
        };

        problemDetails.Extensions.Add(
            "traceId", context.HttpContext.TraceIdentifier);

        return new UnprocessableEntityObjectResult(problemDetails)
        {
            ContentTypes = ["application/problem+json"]
        };
    };
});

builder.Services.AddMapster();

// API�ĵ�����
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1",
        new OpenApiInfo { Title = "MAuth WebAPI", Version = "v1" });

    var securitySchema = new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    options.AddSecurityDefinition("Bearer", securitySchema);
    var securityRequirement = new OpenApiSecurityRequirement
    {
        { securitySchema, ["Bearer"] }
    };
    options.AddSecurityRequirement(securityRequirement);
});

// �������ݿ�֧��
builder.Services.AddDbContextFactory<MAuthDbContext>(options =>
{
    var connectionString =
        builder.Configuration.GetConnectionString("PostgresConnection");
    options.UseNpgsql(connectionString);
});

// ���û���֧��
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration =
        builder.Configuration.GetConnectionString("RedisConnection");
});

// �������ļ��ж�ȡJWT�������
builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection(JwtOptions.Name));

var jwtOptions = builder.Configuration.GetSection(JwtOptions.Name)
    .Get<JwtOptions>()!;

// ��ȡJWT��Կ
var (rsaSecurityPrivateKey, rsaSecurityPublicKey) =
    IdentityHelper.GetPrivateKeyAndPublicKey();

// ʹ��˽Կ��ǩ
builder.Services.AddSingleton(sp => new SigningCredentials(
    rsaSecurityPrivateKey, SecurityAlgorithms.RsaSha256Signature));

// ���幦������
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidAlgorithms = [SecurityAlgorithms.HmacSha256, SecurityAlgorithms.RsaSha256, SecurityAlgorithms.Aes128CbcHmacSha256],

            ValidTypes = [JwtConstants.HeaderType],

            ValidIssuer = jwtOptions.Issuer,

            ValidAudience = jwtOptions.Audience,

            ValidateIssuerSigningKey = true,

            RoleClaimType = ClaimTypes.Role,

            ClockSkew = TimeSpan.Zero,

            IssuerSigningKey = rsaSecurityPublicKey,

            TokenDecryptionKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtOptions.JweEncryptKey)),
        };

        options.SaveToken = true;

        options.EventsType = typeof(AppJwtBearerEvents);
    });

builder.Services.AddScoped<AppJwtBearerEvents>();

// ��Ȩ����
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("Admin", policy => policy.RequireRole(UserRole.Admin.ToString()));

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