using MAuth.Web.Commons.Helpers;
using MAuth.Web.Commons.Options;
using MAuth.Web.Services.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DotNext.Collections.Generic;
using MAuth.Web.Data.Entities;
using Microsoft.AspNetCore.Authorization;

namespace MAuth.Web.Commons.Extensions;

/// <summary>
/// 身份认证配置的扩展方法
/// </summary>
public static class AuthenticationServiceExtensions
{
    /// <summary>
    /// 配置JWT身份认证选项
    /// </summary>
    public static IServiceCollection ConfigureJwtOptions(this IServiceCollection services, IConfiguration configuration)
    {
        return services.Configure<JwtOptions>(
            configuration.GetSection(JwtOptions.Name));
    }

    /// <summary>
    /// 配置JWT签名凭据
    /// </summary>
    public static IServiceCollection ConfigureJwtSigningCredentials(this IServiceCollection services)
    {
        // 获取JWT密钥
        var (rsaSecurityPrivateKey, _) = SecurityHelper.GetPrivateKeyAndPublicKey();
        // 使用私钥加签
        return services.AddSingleton(sp => new SigningCredentials(
            rsaSecurityPrivateKey, SecurityAlgorithms.RsaSha256Signature));
    }

    /// <summary>
    /// 注入身份认证服务
    /// </summary>
    public static IServiceCollection AddConfiguredAuthentication(this IServiceCollection services)
    {
        var (_, rsaSecurityPublicKey) = SecurityHelper.GetPrivateKeyAndPublicKey();

        var jwtOptions = services.BuildServiceProvider().GetRequiredService<IOptions<JwtOptions>>().Value;
        
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = CreateValidationParameters(jwtOptions, rsaSecurityPublicKey);

                options.SaveToken = true;
            });

        return services;
    }

    
    /// <summary>
    /// 授权配置
    /// </summary>
    public static IServiceCollection AddConfiguredAuthorization(this IServiceCollection services)
    {
        services.AddSingleton<IAuthorizationHandler, CurrentUserRequirementHandler>();
        
        services.AddAuthorizationBuilder()
            .AddPolicy(AuthorizationPolicyNames.AdminOnly,
                options => options.RequireRole(nameof(UserRole.Admin)))
            .AddPolicy(AuthorizationPolicyNames.CurrentUserOnly,
                options => options.Requirements.Add(new CurrentUserRequirement()));

        return services;
    }

    /// <summary>
    /// 创建JWT令牌验证参数
    /// </summary>
    /// <param name="jwtOptions">JWT身份认证选项</param>
    /// <param name="rsaSecurityPublicKey">公钥</param>
    /// <returns></returns>
    private static TokenValidationParameters CreateValidationParameters(JwtOptions jwtOptions, RsaSecurityKey rsaSecurityPublicKey)
    {
        return new TokenValidationParameters
        {
            ValidAlgorithms = [
                SecurityAlgorithms.HmacSha256,
                SecurityAlgorithms.RsaSha256,
                SecurityAlgorithms.Aes128CbcHmacSha256
            ],
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
    }
}
