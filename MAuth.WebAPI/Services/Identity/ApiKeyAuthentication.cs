using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;

namespace MAuth.WebAPI.Services.Identity;

public class ApiKeyAuthorizationHandler(IDistributedCache cache, IConfiguration configuration) : AuthorizationHandler<ApiKeyAuthorizationRequirement>
{
    private const string HeaderName = "X-API-KEY";
    
    private readonly IDistributedCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ApiKeyAuthorizationRequirement requirement)
    {
        if (context.Resource is not HttpContext httpContext)
        {
            context.Fail();
            return Task.CompletedTask;
        }

        var reqData = httpContext.Request.Headers[HeaderName];
        var apiKey = configuration.GetValue<string>("ApiKey");

        if (reqData != apiKey)
        {
            context.Fail();
            return Task.CompletedTask;
        }
        context.Succeed(requirement);
        return Task.CompletedTask;
    }
}