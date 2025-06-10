using Mapster;
using MAuth.Data.Entities;
using MAuth.WebAPI.Controllers.Players.DTOs;
using MAuth.WebAPI.Controllers.Players.Parameters;
using MAuth.WebAPI.Services.Identity;
using MAuth.WebAPI.Services.Players;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MAuth.WebAPI.Controllers.Players;

/// <summary>
/// 玩家管理API
/// </summary>
/// <param name="repository"></param>
[ApiController]
[Route("api/players")]
[Authorize(Policy = AuthorizationPolicies.AdminOnly)]
public class PlayersAdminController(PlayerRepository repository) : ControllerBase
{
    private readonly PlayerRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    [HttpGet]
    public async Task<ActionResult<ICollection<Player>>> GetPlayerList([FromQuery] PlayerQueryParameter parameter)
    {
        var players = await _repository.GetPlayerListAsync(parameter);
        return Ok(players.Adapt<ICollection<PlayerDto>>());
    }
}