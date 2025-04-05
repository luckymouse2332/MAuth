using MAuth.Web.Models.Entities;

namespace MAuth.Web.Data.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    /// <summary>
    /// 根据用户名获取用户
    /// </summary>
    /// <param name="username">用户名</param>
    /// <returns></returns>
    Task<User?> GetByUsernameAsync(string username);
}
