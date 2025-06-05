using MAuth.Web.Commons.Models;
using MAuth.Web.Controllers.Users.Parameters;
using MAuth.Web.Data.Entities;

namespace MAuth.Web.Services.Users;

/// <summary>
/// 用户服务
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// 获取用户列表
    /// </summary>
    /// <param name="queryDto">查询参数</param>
    /// <returns>查询到的用户列表</returns>
    Task<PagedList<User>> GetUserListAsync(UserQueryParameters queryDto);

    /// <summary>
    /// 通过ID获取用户信息
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <returns>获取到的用户信息（如果不存在则返回 null ）</returns>
    Task<User?> GetUserByIdAsync(Guid id);

    /// <summary>
    /// 通过用户名获取用户信息
    /// </summary>
    /// <param name="username">用户名</param>
    /// <returns></returns>
    Task<User?> GetUserByUsernameAsync(string username);

    /// <summary>
    /// 添加用户
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    void AddUser(User user);

    /// <summary>
    /// 更新用户信息
    /// </summary>
    /// <param name="user"></param>
    void UpdateUser(User user);

    /// <summary>
    /// 删除用户
    /// </summary>
    /// <param name="user"></param>
    void DeleteUser(User user);

    /// <summary>
    /// 获取指定用户的玩家列表
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>玩家列表</returns>
    Task<ICollection<Player>> GetPlayersForUserAsync(Guid userId);

    /// <summary>
    /// 获取指定用户的指定玩家信息
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="playerId">玩家ID</param>
    /// <returns>单个玩家，如果玩家不存在则为 null </returns>
    Task<Player?> GetPlayerForUserAsync(Guid userId, Guid playerId);

    /// <summary>
    /// 为指定的用户添加一个玩家
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="player">玩家</param>
    /// <returns></returns>
    void AddPlayerForUser(User user, Player player);

    /// <summary>
    /// 保更改到数据库
    /// </summary>
    /// <returns>影响的数据行数（用于判断更改是否成功）</returns>
    Task<int> SaveChangesAsync();
}
