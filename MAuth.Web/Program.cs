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

            // JWT�����֤��������
            // �������ļ��ж�ȡJWT�������
            builder.Services.Configure<JwtOptions>(
                builder.Configuration.GetSection(JwtOptions.Name));
            var jwtOptions = builder.Configuration.GetSection(JwtOptions.Name)
                .Get<JwtOptions>()!;

            // �ǶԳƼ�������
            string privateKeyPath = Path.Combine(builder.Environment.ContentRootPath, "Rsa", "key.private.json");
            string publicKeyPath = Path.Combine(builder.Environment.ContentRootPath, "Rsa", "key.public.json");

            if (!File.Exists(privateKeyPath) || !Path.Exists(publicKeyPath)) // �ж��Ƿ���Ҫ����������Կ�ļ�
            {
                JwtKeyHelper.GenerateKey(builder.Environment);
            }

            var rsaSecurityPrivateKeyString = File.ReadAllText(privateKeyPath);
            var rsaSecurityPublicKeyString = File.ReadAllText(publicKeyPath);
            RsaSecurityKey rsaSecurityPrivateKey = new(JsonConvert.DeserializeObject<RSAParameters>(rsaSecurityPrivateKeyString));
            RsaSecurityKey rsaSecurityPublicKey = new(JsonConvert.DeserializeObject<RSAParameters>(rsaSecurityPublicKeyString));

            // �¼�������
            builder.Services.AddScoped<AppJwtBearerEvents>();

            // ʹ��˽Կ��ǩ
            builder.Services.AddSingleton(sp => new SigningCredentials(rsaSecurityPrivateKey, SecurityAlgorithms.RsaSha256Signature));

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
