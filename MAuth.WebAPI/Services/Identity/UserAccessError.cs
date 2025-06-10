namespace MAuth.WebAPI.Services.Identity;

/// <summary>
/// 用户访问错误
/// </summary>
public enum UserAccessError
{
    /// <summary>
    /// 用户不存在
    /// </summary>
    UserNotExists = 1,

    /// <summary>
    /// 密码错误
    /// </summary>
    InvalidPassword = 2,

    /// <summary>
    /// 令牌无效
    /// </summary>
    InvalidAccessToken = 3,

    /// <summary>
    /// 刷新令牌无效
    /// </summary>
    InvalidRefreshToken = 4
}
