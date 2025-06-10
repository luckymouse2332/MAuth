using DotNext.Collections.Generic;
using MAuth.Data;
using MAuth.Data.Entities;
using MAuth.WebAPI.Commons.Extensions;
using MAuth.WebAPI.Commons.Helpers;
using MAuth.WebAPI.Commons.Models;
using MAuth.WebAPI.Controllers.Players.Parameters;
using MAuth.WebAPI.Controllers.Users.Parameters;
using Microsoft.EntityFrameworkCore;

namespace MAuth.WebAPI.Services.Users;

public class UserRepository(MAuthDbContext dbContext) : IUserRepository
{
    private readonly MAuthDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    public async Task<int> SaveChangesAsync() => await _dbContext.SaveChangesAsync();

    public async Task<PagedList<User>> GetUserListAsync(UserQueryParameters parameter)
    {
        var query = _dbContext.Users
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(parameter.SearchTerm))
        {
            query = query.Where(x => x.Username.Contains(parameter.SearchTerm));
        }

        if (!string.IsNullOrWhiteSpace(parameter.Status)
            && Enum.TryParse(parameter.Status, out UserStatus status))
        {
            query = query.Where(x => x.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(parameter.Role)
            && Enum.TryParse(parameter.Role, out UserRole role))
        {
            query = query.Where(x => x.Role == role);
        }

        if (!string.IsNullOrWhiteSpace(parameter.OrderBy))
        {
            ParameterHelper.GetSorting(parameter.OrderBy).ForEach(x => 
                query = query.ApplySorting(x.Key, x.Value == "desc"));
        }
        
        return await query.ToPagedListAsync(parameter.Page, parameter.PageSize);
    }
    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Username == username);
    }

    public void AddUser(User user)
    {
        user.Password = HashHelper.ComputeSha256Hash(user.Password); // 密码加密
        _dbContext.Users.Add(user);
    }

    public void DeleteUser(User user)
    {
        _dbContext.Users.Remove(user);
    }

    public void UpdateUser(User user)
    {
        user.Password = HashHelper.ComputeSha256Hash(user.Password);
    }

    public async Task<ICollection<Player>> GetPlayersForUserAsync(Guid userId, PlayerQueryParameter parameter)
    {
        var query = _dbContext.Players
            .AsNoTracking()
            .Where(x => x.UserId == userId);
        
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

    public async Task<Player?> GetPlayerForUserAsync(Guid userId, Guid playerId)
    {
        return await _dbContext.Players
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId && x.Id == playerId);
    }

    public void AddPlayerForUser(User user, Player player)
    {
        user.Players ??= [];
        user.Players.Add(player);
    }
}
