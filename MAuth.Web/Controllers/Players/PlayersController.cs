using Microsoft.AspNetCore.Mvc;
using MAuth.Web.Controllers.Players.Requests;
using MAuth.Web.Controllers.Players.DTOs;
using MAuth.Web.Services.Users;
using MAuth.Web.Commons.Binders;
using Mapster;
using MAuth.Web.Data.Entities;
using MapsterMapper;
using MAuth.Web.Services.Identity;
using MAuth.Web.Services.Players;
using Microsoft.AspNetCore.Authorization;

namespace MAuth.Web.Controllers.Players;

[Route("api/users/{userId:guid}/players")]
[ApiController]
[Authorize(Policy = AuthorizationPolicyNames.CurrentUserOnly)]
public class PlayersController(IPlayerRepository playerRepository, IUserRepository userRepository, IMapper mapper) : ControllerBase
{
    private readonly IPlayerRepository _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
    private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    [HttpGet]
    public async Task<ActionResult<ICollection<PlayerDto>>> GetPlayersForUser(Guid userId)
    {
        var results = await _userRepository.GetPlayersForUserAsync(userId);
        return Ok(results.Adapt<ICollection<PlayerDto>>());
    }

    [HttpGet("{playerId:guid}", Name = nameof(GetPlayerForUser))]
    public async Task<ActionResult<PlayerDto>> GetPlayerForUser(Guid userId, Guid playerId)
    {
        var result = await _userRepository.GetPlayerForUserAsync(userId, playerId);
        return Ok(result.Adapt<PlayerDto>());
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
