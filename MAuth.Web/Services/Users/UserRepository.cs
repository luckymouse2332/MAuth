using MAuth.Web.Commons.Helpers;
using MAuth.Web.Commons.Models;
using MAuth.Web.Controllers.Users.Parameters;
using MAuth.Web.Data;
using MAuth.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace MAuth.Web.Services.Users;

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

        query = query.Skip(parameter.PageSize * (parameter.Page - 1))
            .Take(parameter.PageSize);

        return await PagedList<User>
            .CreateAsync(query, parameter.Page, parameter.PageSize);
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

    public async Task<ICollection<Player>> GetPlayersForUserAsync(Guid userId)
    {
        return await _dbContext.Players
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .ToListAsync();
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
