using Mapster;
using MapsterMapper;
using MAuth.Web.Commons.Binders;
using MAuth.Web.Controllers.Users.DTOs;
using MAuth.Web.Controllers.Users.Parameters;
using MAuth.Web.Controllers.Users.Requests;
using MAuth.Web.Data.Entities;
using MAuth.Web.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using System.Text.Json;
using MAuth.Web.Services.Identity;

namespace MAuth.Web.Controllers.Users;

/// <summary>
/// 用户管理API
/// </summary>
[Route("api/users")]
[ApiController]
[Authorize(Policy = AuthorizationPolicyNames.AdminOnly)]
public class UserAdminController(IUserRepository userRepository, IMapper mapper) : ControllerBase
{
    private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    /// <summary>
    /// 获取用户列表
    /// </summary>
    /// <returns>用户列表</returns>
    [HttpGet]
    public async Task<ActionResult<ICollection<UserDto>>> GetUserList(
        [FromQuery] UserQueryParameters queryDto)
    {
        var users = await _userRepository.GetUserListAsync(queryDto);

        var paginationMetadata = new
        {
            totalCount = users.TotalCount,
            pageSize = users.PageSize,
            currentPage = users.CurrentPage,
            totalPages = users.TotalPages
        };

        Response.Headers.Append(
            "X-Pagination", JsonSerializer.Serialize(paginationMetadata, JsonOptions));

        return Ok(users.Adapt<ICollection<UserDto>>());
    }

    /// <summary>
    /// 根据ID获取单个用户
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>单个用户</returns>
    [HttpGet("{userId:guid}", Name = nameof(GetUserById))]
    public async Task<ActionResult<UserDto>> GetUserById(Guid userId)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);
        return Ok(user.Adapt<UserDto>());
    }

    /// <summary>
    /// 添加用户
    /// </summary>
    /// <param name="dto">要添加的用户</param>
    /// <param name="modifier">自动注入修改者信息</param>
    /// <returns>添加后的用户</returns>
    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateUser(
        UserCreateRequest dto, [FromUsername] string modifier)
    {
        var user = dto.Adapt<User>();

        user.Modifier = modifier;

        _userRepository.AddUser(user);

        await _userRepository.SaveChangesAsync();

        var result = user.Adapt<UserDto>();

        return CreatedAtRoute(nameof(GetUserById), new
        {
            id = result.Id,
        }, result);
    }

    /// <summary>
    /// 根据ID更新用户（整体替换）
    /// </summary>
    /// <param name="userId">要更新的用户ID</param>
    /// <param name="dto">要更新的目标</param>
    [HttpPut("{userId:guid}")]
    public async Task<ActionResult> UpdateUser(
        Guid userId, UserUpdateRequest dto, [FromUsername] string modifier)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);

        if (user == null)
        {
            return NotFound("用户不存在！");
        }

        _mapper.Map(dto, user);

        user.Modifier = modifier;

        _userRepository.UpdateUser(user);

        await _userRepository.SaveChangesAsync();

        return NoContent();
    }
    
    /// <summary>
    /// 根据ID删除用户
    /// </summary>
    /// <param name="userId">要删除的用户ID</param>
    /// <param name="modifier">自动注入修改者信息</param>
    [HttpDelete("{userId:guid}")]
    public async Task<ActionResult> DeleteUser(Guid userId, [FromUsername] string modifier)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);

        if (user == null)
        {
            return NotFound("用户不存在！");
        }

        user.Modifier = modifier;
        _userRepository.DeleteUser(user);

        await _userRepository.SaveChangesAsync();

        return NoContent();
    }
}
