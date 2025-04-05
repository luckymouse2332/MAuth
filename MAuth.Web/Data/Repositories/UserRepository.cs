using MAuth.Web.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MAuth.Web.Data.Repositories;

public class UserRepository(MAuthDbContext dbContext)
    : BaseRepository<User>(dbContext), IUserRepository
{
    private readonly MAuthDbContext _dbContext = dbContext;

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _dbContext.Users.Where(x => x.Username == username)
            .FirstOrDefaultAsync();
    }
}

