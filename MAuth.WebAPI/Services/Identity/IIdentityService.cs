using DotNext;
using MAuth.Data.Entities;

namespace MAuth.WebAPI.Services.Identity;

/// <summary>
/// 身份认证服务
/// </summary>
public interface IIdentityService
{
    /// <summary>
    /// 用户登录校验
    /// </summary>
    /// <param name="username">用户名</param>
    /// <param name="password">用户密码</param>
    /// <returns>校验结果</returns>
    Task<Result<User, UserAccessError>> VerifyLoginDataAsync(string username, string password);

    /// <summary>
    /// 校验令牌数据
    /// </summary>
    /// <param name="token">提交的令牌数据</param>
    /// <returns>刷新完成的令牌</returns>
    Task<Result<User, UserAccessError>> ValidateTokenAsync(string accessToken, string refreshToken);

    /// <summary>
    /// 创建所需的令牌
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    Task<(string accessToken, string refreshToken)> CreateTokenAsync(User user);
}
