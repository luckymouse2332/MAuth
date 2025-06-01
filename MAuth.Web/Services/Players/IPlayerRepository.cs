using MAuth.Web.Controllers.Players.Parameters;
using MAuth.Web.Data.Entities;

namespace MAuth.Web.Services.Players;

/// <summary>
/// 玩家服务接口
/// </summary>
public interface IPlayerRepository
{
    /// <summary>
    /// 查询玩家列表
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    Task<ICollection<Player>> GetPlayerListAsync(PlayerQueryParameter parameter);

    /// <summary>
    /// 更新玩家（因为不涉及到用户数据，所以在玩家服务中处理）
    /// </summary>
    /// <param name="player"></param>
    void UpdatePlayer(Player player);

    /// <summary>
    /// 删除玩家（因为不涉及到用户数据，所以在玩家服务中处理）
    /// </summary>
    /// <param name="player"></param>
    void DeletePlayer(Player player);

    /// <summary>
    /// 保存更改（只要有修改数据的方法就要有这个）
    /// </summary>
    /// <returns></returns>
    Task<int> SaveChangesAsync();
}
