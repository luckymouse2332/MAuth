using Mapster;
using MAuth.Web.Commons.Helpers;
using MAuth.Web.Controllers.Users.DTOs;
using MAuth.Web.Controllers.Users.Requests;
using MAuth.Web.Services.Identity;
using MAuth.Web.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MAuth.Web.Controllers.Users;

/// <summary>
/// 身份认证控制器
/// </summary>
[Route("api/auth")]
[ApiController]
public class UserController(IIdentityService identityService, IUserRepository userRepository) : ControllerBase
{
    private readonly IIdentityService _identityService =
        identityService ?? throw new ArgumentNullException(nameof(identityService));

    private readonly IUserRepository _userRepository =
        userRepository ?? throw new ArgumentNullException(nameof(userRepository));

    /// <summary>
    /// 登录
    /// </summary>
    /// <param name="loginRequest"></param>
    /// <returns></returns>
    [HttpPost("login")]
    public async Task<ActionResult<AuthTokenDto>> Login(UserLoginRequest loginRequest)
    {
        var result = await _identityService.VerifyLoginDataAsync(loginRequest.Username, loginRequest.Password);
        if (!result.IsSuccessful)
        {
            return result.Error switch
            {
                UserAccessError.UserNotExists => NotFound("用户不存在"),
                UserAccessError.InvalidPassword => Unauthorized("密码错误"),
                _ => BadRequest("登录失败")
            };
        }

        var (accessToken, refreshToken) = await _identityService.CreateTokenAsync(result.Value);

        return new AuthTokenDto()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    /// <summary>
    /// 刷新 token
    /// </summary>
    /// <returns></returns>
    [HttpPost("refresh")]
    public async Task<ActionResult<AuthTokenDto>> RefreshToken(AuthTokenDto authToken)
    {
        var result = await _identityService.ValidateTokenAsync(authToken.AccessToken, authToken.RefreshToken);

        if (!result.IsSuccessful)
        {
            return result.Error switch
            {
                UserAccessError.InvalidAccessToken => Unauthorized("认证令牌无效！"),
                UserAccessError.InvalidRefreshToken => Unauthorized("刷新令牌无效！"),
                UserAccessError.UserNotExists => NotFound("用户不存在！"),
                _ => BadRequest("刷新令牌失败！")
            };
        }

        var (accessToken, refreshToken) = await _identityService.CreateTokenAsync(result.Value);

        return new AuthTokenDto()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    [Authorize]
    [HttpGet("info")]
    public async Task<ActionResult<UserDto>> GetUserInfo()
    {
        var id = IdentityHelper.GetUserIdFromPrincipal(User);

        if (!id.HasValue)
        {
            return Unauthorized("未登录或登录已过期");
        }

        var user = await _userRepository.GetUserByIdAsync(id.Value);

        if (user is null)
        {
            return NotFound("用户不存在");
        }

        return Ok(user.Adapt<UserDto>());
    }
}