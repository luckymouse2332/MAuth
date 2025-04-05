using MAuth.Web.Models.DTOs;
using MAuth.Web.Services;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<ActionResult<string>> LoginAsync(UserLoginDto loginDto)
        {
            return await _authService.LoginAsync(loginDto.Username, loginDto.Password);
        }
    }
}
