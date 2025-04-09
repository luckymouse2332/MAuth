using MAuth.Web.Models.DTOs;
using System.Security.Claims;

namespace MAuth.Web.Services
{
    /// <summary>
    /// 身份认证服务
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">用户密码</param>
        /// <returns>JWT Token</returns>
        Task<AuthTokenDto> LoginAsync(string username, string password);

        /// <summary>
        /// 从用户信息创建用于认证的JWT字符串和用于刷新令牌的刷新token字符串
        /// </summary>
        /// <param name="user">要包含的用户信息</param>
        /// <returns>身份认证令牌</returns>
        Task<AuthTokenDto> CreateAuthTokenAsync(UserDto user);

        /// <summary>
        /// 刷新身份令牌
        /// </summary>
        /// <param name="token"></param>
        /// <returns>刷新完成的令牌</returns>
        Task<AuthTokenDto> RefreshAuthTokenAsync(AuthTokenDto token);

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<UserDto> GetUserInfoAsync(ClaimsPrincipal user);
    }
}
