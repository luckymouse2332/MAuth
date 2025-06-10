using DotNext.Collections.Generic;
using MAuth.Data;
using MAuth.Data.Entities;
using MAuth.WebAPI.Commons.Extensions;
using MAuth.WebAPI.Commons.Helpers;
using MAuth.WebAPI.Controllers.Players.Parameters;
using Microsoft.EntityFrameworkCore;

namespace MAuth.WebAPI.Services.Players;

public class PlayerRepository(MAuthDbContext dbContext) : IPlayerRepository
{
    private readonly MAuthDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

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
        
        if (!string.IsNullOrWhiteSpace(parameter.Status) 
            && Enum.TryParse(parameter.Status, out PlayerStatus status))
        {
            query = query.Where(p => p.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(parameter.OrderBy)) // 添加排序
        {
            ParameterHelper.GetSorting(parameter.OrderBy).ForEach(x =>
                query = query.ApplySorting(x.Key, x.Value == "desc"));
        }
        
        return await query.ToPagedListAsync(parameter.Page, parameter.Page);
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
