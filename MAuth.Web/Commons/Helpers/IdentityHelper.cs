using MAuth.Web.Services.Identity;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;

namespace MAuth.Web.Commons.Helpers;

/// <summary>
/// 提供了身份认证相关帮助方法
/// </summary>
public static class IdentityHelper
{
    /// <summary>
    /// 获取用于存储用户刷新令牌的缓存键。
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="refreshTokenId">刷新令牌的ID</param>
    /// <returns>生成好的缓存键</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static string GetRefreshTokenKey(string userId, string refreshTokenId)
    {
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentNullException(nameof(userId));
        if (string.IsNullOrWhiteSpace(refreshTokenId)) throw new ArgumentNullException(nameof(refreshTokenId));

        return $"{userId}:{refreshTokenId}";
    }

    /// <summary>
    /// 生成随机的刷新令牌字符串。
    /// </summary>
    /// <returns></returns>
    public static string GenerateRefreshToken()
    {
        using var rng = RandomNumberGenerator.Create();
        var randomBytes = new byte[32]; // 256 bits
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }


    /// <summary>
    /// 从ClaimsPrincipal中获取用户ID。
    /// </summary>
    /// <param name="user">ClaimsPrincipal</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static Guid? GetUserIdFromPrincipal(ClaimsPrincipal user)
    {
        ArgumentNullException.ThrowIfNull(user, nameof(user));

        var userIdClaim = user.Claims.FirstOrDefault(c => c.Type == JwtOptions.UserIdClaimType)
            ?? throw new InvalidOperationException("User ID claim not found.");

        if (!Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return null;
        }

        return userId;
    }

    /// <summary>
    /// 生成用于JWT非对称加密的公钥和私钥
    /// </summary>
    private static void GenerateKeys()
    {
        using var rsa = new RSACryptoServiceProvider(2048);

        var privateKey = rsa.ExportParameters(true);

        var publicKey = rsa.ExportParameters(false);

        if (!Directory.Exists(Dir))
        {
            Directory.CreateDirectory(Dir);
            Log.Information("Created a new directory to save keys: {0}", Dir);
        }

        File.WriteAllText(PrivateKeyPath, JsonSerializer.Serialize(privateKey, options));

        File.WriteAllText(PublicKeyPath, JsonSerializer.Serialize(publicKey, options));

        Log.Information("Keys are created successfully!");

        // 及时销毁，以免出现密钥泄露问题
        rsa.PersistKeyInCsp = false;
    }

    /// <summary>
    /// 获取用于JWT非对称加密的私钥和公钥。
    /// </summary>
    /// <returns></returns>
    public static (RsaSecurityKey privateKey, RsaSecurityKey publicKey)
        GetPrivateKeyAndPublicKey()
    {
        if (!File.Exists(PrivateKeyPath)
            || !Path.Exists(PublicKeyPath)) // 判断是否需要重新生成密钥文件
        {
            GenerateKeys();
        }

        var privateKeyString = File.ReadAllText(PrivateKeyPath);
        var publicKeyString = File.ReadAllText(PublicKeyPath);

        var rsaSecurityPrivateKey = new RsaSecurityKey(JsonSerializer.Deserialize<RSAParameters>(privateKeyString, options));

        var rsaSecurityPublicKey = new RsaSecurityKey(JsonSerializer.Deserialize<RSAParameters>(publicKeyString, options));

        return (rsaSecurityPrivateKey, rsaSecurityPublicKey);
    }

    private static string Dir =>
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Rsa");

    private static string PrivateKeyPath =>
        Path.Combine(Dir, "key.private.json");

    private static string PublicKeyPath =>
        Path.Combine(Dir, "key.public.json");

    private static JsonSerializerOptions options = new()
    {
        IncludeFields = true
    };
}
