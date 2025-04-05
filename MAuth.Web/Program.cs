using MAuth.Web.Configurations;
using MAuth.Web.Controllers.Filters;
using MAuth.Web.Data;
using MAuth.Web.Data.Repositories;
using MAuth.Web.Helpers;
using MAuth.Web.Helpers.Logging;
using MAuth.Web.Mappings;
using MAuth.Web.Middlewares;
using MAuth.Web.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.Text;
using MAuth.Web.Models.Entities;

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

            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });

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

                    problemDetails.Extensions.Add("traceId", context.HttpContext.TraceIdentifier);

                    return new UnprocessableEntityObjectResult(problemDetails)
                    {
                        ContentTypes = ["application/problem+json"]
                    };
                };
            });

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

            // JWT身份认证功能配置
            // 从配置文件中读取JWT相关配置
            builder.Services.Configure<JwtOptions>(
                builder.Configuration.GetSection(JwtOptions.Name));
            var jwtOptions = builder.Configuration.GetSection(JwtOptions.Name)
                .Get<JwtOptions>()!;

            // 非对称加密配置
            string privateKeyPath = Path.Combine(builder.Environment.ContentRootPath, "Rsa", "key.private.json");
            string publicKeyPath = Path.Combine(builder.Environment.ContentRootPath, "Rsa", "key.public.json");

            if (!File.Exists(privateKeyPath) || !Path.Exists(publicKeyPath)) // 判断是否需要重新生成密钥文件
            {
                JwtKeyHelper.GenerateKey(builder.Environment);
            }

            var rsaSecurityPrivateKeyString = File.ReadAllText(privateKeyPath);
            var rsaSecurityPublicKeyString = File.ReadAllText(publicKeyPath);
            RsaSecurityKey rsaSecurityPrivateKey = new(JsonConvert.DeserializeObject<RSAParameters>(rsaSecurityPrivateKeyString));
            RsaSecurityKey rsaSecurityPublicKey = new(JsonConvert.DeserializeObject<RSAParameters>(rsaSecurityPublicKeyString));

            // 事件处理器
            builder.Services.AddScoped<AppJwtBearerEvents>();

            // 使用私钥加签
            builder.Services.AddSingleton(sp => new SigningCredentials(rsaSecurityPrivateKey, SecurityAlgorithms.RsaSha256Signature));

            // 主体功能配置
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

                        NameClaimType = ClaimTypes.Name,

                        RoleClaimType = ClaimTypes.Role,


                        ClockSkew = TimeSpan.Zero,

                        IssuerSigningKey = rsaSecurityPublicKey,

                        TokenDecryptionKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtOptions.JweEncryptKey)),
                    };

                    options.SaveToken = true;

                    options.EventsType = typeof(AppJwtBearerEvents);
                });

            builder.Services.AddScoped<IAuthService, AuthService>();

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy => policy.RequireRole(UserRole.Admin.ToString()));
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseGlobalExceptionHandler();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseRouting();

            app.MapControllers();

            app.Run();
        }
    }
}
