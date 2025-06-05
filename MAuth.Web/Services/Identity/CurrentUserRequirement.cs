using Microsoft.AspNetCore.Authorization;

namespace MAuth.Web.Services.Identity;

/// <summary>
/// 限制用户访问资源范围的身份认证策略配置
/// </summary>
/// <remarks>仅允许当前用户访问自己Id对应的资源</remarks>
public class CurrentUserRequirement : IAuthorizationRequirement
{
}