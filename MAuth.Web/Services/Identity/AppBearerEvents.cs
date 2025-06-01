using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Net.Http.Headers;

namespace MAuth.Web.Services.Identity;

public class AppJwtBearerEvents : JwtBearerEvents
{
    public override Task MessageReceived(MessageReceivedContext context)
    {
        // 从 Http Request Header 中获取 Authorization
        string? authorization = context.Request.Headers[HeaderNames.Authorization];
        if (string.IsNullOrWhiteSpace(authorization))
        {
            context.NoResult();
            return Task.CompletedTask;
        }

        // 必须为 Bearer 认证方案
        if (authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            // 赋值 token
            context.Token = authorization["Bearer ".Length..].Trim();
        }

        if (string.IsNullOrWhiteSpace(context.Token))
        {
            context.NoResult();
            return Task.CompletedTask;
        }

        return Task.CompletedTask;
    }
}
