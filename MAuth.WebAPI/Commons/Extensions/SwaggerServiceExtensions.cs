using MAuth.WebAPI.Commons.Options;
using Microsoft.OpenApi.Models;

namespace MAuth.WebAPI.Commons.Extensions;

/// <summary>
/// 配置API文档（Swagger）服务
/// </summary>
public static class SwaggerServiceExtensions
{
    /// <summary>
    /// 添加Swagger服务配置
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddConfiguredSwagger(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEndpointsApiExplorer();

        var options = configuration.GetSection("Swagger").Get<ApplicationSwaggerOptions>() ?? new();

        var basicOptions = options.BasicOptions;
        var securityOptions = options.SecurityOptions;

        var info = CreateOpenApiInfo(basicOptions);
        var scheme = CreateOpenApiSecurityScheme(securityOptions);

        services.AddSwaggerGen(swagger =>
        {
            swagger.SwaggerDoc(basicOptions.Version, info);

            swagger.AddSecurityDefinition(securityOptions.SchemeName, scheme);
            swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                [scheme] = [securityOptions.SchemeName]
            });
        });

        return services;
    }

    /// <summary>
    /// 创建OpenAPI信息配置
    /// </summary>
    /// <param name="options">信息选项，包括描述、标题、版本信息</param>
    private static OpenApiInfo CreateOpenApiInfo(SwaggerInfoOptions options) => new()
    {
        Title = options.Title,
        Version = options.Version,
        Description = options.Description,
    };

    /// <summary>
    /// 创建OpenAPI安全方案配置
    /// </summary>
    /// <param name="options">安全方案选项，包括方案、描述信息</param>
    private static OpenApiSecurityScheme CreateOpenApiSecurityScheme(SwaggerSecurityOptions options) => new()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = options.SchemeName,
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = options.SchemeDescription,

        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = options.SchemeName
        }
    };
}
