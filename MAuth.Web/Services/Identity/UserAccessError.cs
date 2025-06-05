namespace MAuth.Web.Services.Identity;

/// <summary>
/// 用户访问错误
/// </summary>
public enum UserAccessError
{
    /// <summary>
    /// 用户不存在
    /// </summary>
    UserNotExists,

    /// <summary>
    /// 密码错误
    /// </summary>
    InvalidPassword,

    /// <summary>
    /// 令牌无效
    /// </summary>
    InvalidAccessToken,

    /// <summary>
    /// 刷新令牌无效
    /// </summary>
    InvalidRefreshToken
}
