using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Net.Http.Headers;
using Serilog;

namespace MAuth.Web.Services
{
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
                // 赋值token
                context.Token = authorization["Bearer ".Length..].Trim();
            }

            if (string.IsNullOrWhiteSpace(context.Token))
            {
                context.NoResult();
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public override Task AuthenticationFailed(AuthenticationFailedContext context)
        {
            Log.Error($"Exception: {context.Exception}");

            return Task.CompletedTask;
        }

        public override Task Challenge(JwtBearerChallengeContext context)
        {
            Log.Error($"Authenticate Failure: {context.AuthenticateFailure}");
            Log.Error($"Error: {context.Error}");
            Log.Error($"Error Description: {context.ErrorDescription}");
            Log.Error($"Error Uri: {context.ErrorUri}");

            return Task.CompletedTask;
        }
    }
}
