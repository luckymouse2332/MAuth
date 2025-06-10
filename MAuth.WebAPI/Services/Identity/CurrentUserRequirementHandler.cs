using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace MAuth.WebAPI.Services.Identity;

/// <summary>
/// 实现了对用户访问域的限制
/// </summary>
public class CurrentUserRequirementHandler : AuthorizationHandler<CurrentUserRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CurrentUserRequirement requirement)
    {
        if (context.Resource is not HttpContext httpContext)
        {
            context.Fail();
            return Task.CompletedTask;
        }
        
        // 从路由参数中拿id
        var routeId = httpContext.GetRouteValue("userId")?.ToString();
        if (string.IsNullOrEmpty(routeId))
        {
            context.Fail();
            return Task.CompletedTask;
        }

        // 当前用户id
        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId != null && userId == routeId)
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }

        return Task.CompletedTask;
    }
}