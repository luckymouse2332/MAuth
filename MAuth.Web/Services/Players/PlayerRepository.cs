using MAuth.Contracts.Enums;
using MAuth.Web.Commons.Helpers;
using MAuth.Web.Commons.Models;
using MAuth.Web.Controllers.Players.Parameters;
using MAuth.Web.Data;
using MAuth.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace MAuth.Web.Services.Players;

public class PlayerRepository(MAuthDbContext dbContext) : IPlayerRepository
{
    private readonly MAuthDbContext _dbContext = dbContext;

    public async Task<int> SaveChangesAsync() => await _dbContext.SaveChangesAsync();

    public async Task<ICollection<Player>> GetPlayerListAsync(PlayerQueryParameter parameter)
    {
        var query = _dbContext.Players
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrEmpty(parameter.SearchTerm))
        {
            query = query.Where(p => p.Name.Contains(parameter.SearchTerm));
        }

        if (parameter.PageSize > 0)
        {
            query = query.Skip((parameter.Page - 1) * parameter.PageSize)
                         .Take(parameter.PageSize);
        }

        var status = EnumHelper.ParseEnum<PlayerStatus>(parameter.Status);

        if (status != null)
        {
            query = query.Where(p => p.Status == status);
        }

        return await PagedList<Player>.CreateAsync(query, parameter.Page, parameter.PageSize);
    }

    public void UpdatePlayer(Player player)
    {
        _dbContext.Players.Update(player);
    }

    public void DeletePlayer(Player player)
    {
        _dbContext.Players.Remove(player);
    }
}
