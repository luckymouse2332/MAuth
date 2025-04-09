using MAuth.Web.Models.DTOs;
using MAuth.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Principal;

namespace MAuth.Web.Controllers
{
    /// <summary>
    /// 身份认证控制器
    /// </summary>
    /// <param name="authService"></param>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="loginDto"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<ActionResult<AuthTokenDto>> LoginAsync(UserLoginDto loginDto)
        {
            return await _authService.LoginAsync(loginDto.Username, loginDto.Password);
        }

        /// <summary>
        /// 刷新token
        /// </summary>
        /// <returns></returns>
        [HttpPost("refresh")]
        public async Task<ActionResult<AuthTokenDto>> RefreshTokenAsync(AuthTokenDto authToken)
        {
            return await _authService.RefreshAuthTokenAsync(authToken);
        }

        [Authorize]
        [HttpGet("info")]
        public async Task<ActionResult<UserDto>> GetUserInfoAsync()
        {
            return await _authService.GetUserInfoAsync(HttpContext.User);
        }
    }
}
