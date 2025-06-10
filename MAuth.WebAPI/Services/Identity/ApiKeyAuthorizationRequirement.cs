using Microsoft.AspNetCore.Authorization;

namespace MAuth.WebAPI.Services.Identity;

/// <summary>
/// API Key认证方案
/// </summary>
public class ApiKeyAuthorizationRequirement : IAuthorizationRequirement
{
}