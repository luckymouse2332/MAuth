using MAuth.WebAPI.Commons.Helpers;
using MAuth.WebAPI.Controllers.Velocity.Requests;
using MAuth.WebAPI.Services.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace MAuth.WebAPI.Controllers.Velocity;

/// <summary>
/// 与Velocity代理端通信
/// </summary>
[ApiController]
[Route("api/velocity")]
[Authorize(Policy = AuthorizationPolicies.RequireApiKey)]
public class VelocityController(IDistributedCache cache) : ControllerBase
{
    private readonly IDistributedCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));

    [HttpGet("status")]
    public async Task<ActionResult<bool>> GetPlayerLoginStatus([FromQuery] PlayerLoginCheckRequest request)
    {
        return await _cache.GetAsync(PlayerLoginHelper.GetPlayerCacheKey(request.IpAddress, request.UUID)) != null;
    }
}