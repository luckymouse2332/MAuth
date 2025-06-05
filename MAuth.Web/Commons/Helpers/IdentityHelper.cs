using MAuth.Web.Commons.Options;
using System.Security.Claims;
using System.Security.Cryptography;

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

        var userIdClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("User ID claim not found.");

        if (!Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return null;
        }

        return userId;
    }
}
