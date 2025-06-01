using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MAuth.Web.Services.Users;
using MAuth.Web.Commons.Models;
using MAuth.Web.Controllers.Users.Requests;
using MAuth.Web.Controllers.Users.DTOs;
using MAuth.Web.Controllers.Users.Parameters;
using MAuth.Web.Commons.Binders;
using Mapster;
using MAuth.Web.Data.Entities;
using MapsterMapper;

namespace MAuth.Web.Controllers.Users;

/// <summary>
/// 用户管理API
/// </summary>
[Route("api/users")]
[ApiController]
[Authorize(Policy = "Admin")]
public class UserAdminController(IUserRepository userRepository, IMapper mapper) : ControllerBase
{
    private readonly IUserRepository _userRepository = userRepository;

    private readonly IMapper _mapper = mapper;

    /// <summary>
    /// 获取用户列表
    /// </summary>
    /// <returns>用户列表</returns>
    [HttpGet]
    public async Task<ActionResult<PagedList<UserDto>>> GetUserList(
        [FromQuery] UserQueryParameters queryDto)
    {
        var users = await _userRepository.GetUserListAsync(queryDto);
        return Ok(users);
    }

    /// <summary>
    /// 根据ID获取单个用户
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <returns>单个用户</returns>
    [HttpGet("{userId:guid}", Name = nameof(GetUserById))]
    public async Task<ActionResult<UserDto>> GetUserById(Guid userId)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);
        return Ok(user);
    }

    /// <summary>
    /// 添加用户
    /// </summary>
    /// <param name="dto">要添加的用户</param>
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
    /// <param name="id">要更新的用户ID</param>
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
    /// <param name="id">要删除的用户ID</param>
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
