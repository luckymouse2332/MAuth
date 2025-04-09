using AutoMapper;
using BCrypt.Net;
using MAuth.Web.Data.Repositories;
using MAuth.Web.Models.DTOs;
using MAuth.Web.Models.Entities;
using MAuth.Web.Models.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MAuth.Web.Controllers
{
    /// <summary>
    /// 用户管理API
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(IUserRepository userRepository, IMapper mapper) : ControllerBase
    {

        private readonly IUserRepository _userRepository = userRepository;

        private readonly IMapper _mapper = mapper;

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <returns>用户列表</returns>
        [HttpGet]
        [Authorize(Policy = "Admin")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUserListAsync()
        {
            var users = await _userRepository.GetAsync();
            var userDtos = _mapper.Map<IEnumerable<UserDto>>(users);
            return Ok(userDtos);
        }

        /// <summary>
        /// 根据ID获取单个用户
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns>单个用户</returns>
        [HttpGet("{id:guid}", Name = nameof(GetUserByIdAsync))]
        [Authorize(Policy = "Admin")]
        public async Task<ActionResult<UserDto>> GetUserByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            var userDto = _mapper.Map<UserDto>(user);
            return Ok(userDto);
        }

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="user">要添加的用户</param>
        /// <returns>添加后的用户</returns>
        [HttpPost]
        [Authorize(Policy = "Admin")]
        public async Task<ActionResult<UserDto>> CreateUserAsync(UserCreateDto user)
        {
            var userToAdd = _mapper.Map<User>(user);
            userToAdd.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(userToAdd.Password);
            
            await _userRepository.AddAsync(userToAdd);

            var dtoToReturn = _mapper.Map<UserDto>(userToAdd);

            return CreatedAtRoute(nameof(GetUserByIdAsync), new
            {
                id = userToAdd.Id,
            }, dtoToReturn);
        }

        /// <summary>
        /// 根据ID更新用户（整体替换）
        /// </summary>
        /// <param name="id">要更新的用户ID</param>
        /// <param name="user">要更新的目标</param>
        [HttpPut("{id:guid}")]
        [Authorize(Policy = "Admin")]
        public async Task<ActionResult> UpdateUserAsync(Guid id, [FromBody] UserUpdateDto user)
        {
            if (!await _userRepository.ExistsAsync(id))
            {
                throw new CustomException(400, $"不存在ID为{id}的用户！");
            }

            var userEntity = _mapper.Map<User>(user);
            userEntity.Id = id;
            if (!string.IsNullOrWhiteSpace(user.Password))
            {
                userEntity.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(user.Password);
            }
            await _userRepository.UpdateAsync(userEntity);
            
            return NoContent();
        }
        
        /// <summary>
        /// 根据ID删除用户
        /// </summary>
        /// <param name="id">要删除的用户ID</param>
        [HttpDelete("{id:guid}")]
        [Authorize(Policy = "Admin")]
        public async Task<ActionResult> DeleteUserAsync(Guid id)
        {
            if (!await _userRepository.ExistsAsync(id))
            {
                throw new CustomException(400, $"不存在ID为{id}的用户！");
            }

            var user = await _userRepository.GetByIdAsync(id);

            await _userRepository.DeleteAsync(user!);
            return NoContent();
        }
    }
}
