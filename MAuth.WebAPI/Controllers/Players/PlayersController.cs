using System.Text;
using Microsoft.AspNetCore.Mvc;
using Mapster;
using MapsterMapper;
using MAuth.Data.Entities;
using MAuth.WebAPI.Commons.Helpers;
using MAuth.WebAPI.Controllers.Players.DTOs;
using MAuth.WebAPI.Controllers.Players.Parameters;
using MAuth.WebAPI.Controllers.Players.Requests;
using MAuth.WebAPI.Services.Identity;
using MAuth.WebAPI.Services.Players;
using MAuth.WebAPI.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;

namespace MAuth.WebAPI.Controllers.Players;

[Route("api/users/{userId:guid}/players")]
[ApiController]
[Authorize(Policy = AuthorizationPolicies.CurrentUserOnly)]
public class PlayersController(IPlayerRepository playerRepository, IUserRepository userRepository, IMapper mapper, IDistributedCache cache) : ControllerBase
{
    private readonly IPlayerRepository _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
    private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    private readonly IDistributedCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));

    [HttpGet]
    public async Task<ActionResult<ICollection<PlayerDto>>> GetPlayersForUser(Guid userId, [FromQuery] PlayerQueryParameter parameter)
    {
        var results = await _userRepository.GetPlayersForUserAsync(userId, parameter);
        return Ok(results.Adapt<ICollection<PlayerDto>>());
    }

    [HttpGet("{playerId:guid}", Name = nameof(GetPlayerForUser))]
    public async Task<ActionResult<PlayerDto>> GetPlayerForUser(Guid userId, Guid playerId)
    {
        var result = await _userRepository.GetPlayerForUserAsync(userId, playerId);
        return Ok(result.Adapt<PlayerDto>());
    }
    
    [HttpPost("{playerId:guid}")]
    public async Task<ActionResult> PlayerLogin(Guid userId, Guid playerId)
    {
        var player = await _userRepository.GetPlayerForUserAsync(userId, playerId);

        if (player is null) return NotFound("指定玩家不存在！");

        var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString();

        if (clientIp is null) return BadRequest("无法获取客户端IP");
        
        await _cache.SetAsync(PlayerLoginHelper.GetPlayerCacheKey(clientIp, player.UUID), "logged in"u8.ToArray());
        
        return Ok();
    }

    [HttpPost]
    public async Task<ActionResult> CreateForUser(
        Guid userId, PlayerCreateRequest dto, [FromUsername] string modifier)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);

        if (user == null)
        {
            return NotFound("用户不存在！");
        }

        var player = dto.Adapt<Player>();

        player.Modifier = modifier;

        _userRepository.AddPlayerForUser(user, player);

        await _userRepository.SaveChangesAsync();

        var result = player.Adapt<PlayerDto>();

        return CreatedAtRoute(nameof(GetPlayerForUser), new
        {
            userId,
            playerId = result.Id
        }, result);
    }

    [HttpPut("{playerId:guid}")]
    public async Task<ActionResult> UpdatePlayerForUser(
        Guid userId, Guid playerId, PlayerUpdateRequest dto, [FromUsername] string modifier)
    {
        var player = await _userRepository.GetPlayerForUserAsync(userId, playerId);

        if (player == null)
        {
            return NotFound("玩家不存在！");
        }

        _mapper.Map(dto, player);

        player.Modifier = modifier;

        _playerRepository.UpdatePlayer(player);

        await _playerRepository.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{playerId:guid}")]
    public async Task<ActionResult> RemovePlayer(
        Guid userId, Guid playerId, [FromUsername] string modifier)
    {
        var player = await _userRepository.GetPlayerForUserAsync(userId, playerId);

        if (player == null)
        {
            return NotFound("玩家不存在！");
        }

        player.Modifier = modifier;

        _playerRepository.DeletePlayer(player);

        await _playerRepository.SaveChangesAsync();

        return NoContent();
    }
}
