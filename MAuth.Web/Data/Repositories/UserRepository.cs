using MAuth.Web.Data.Entities;

namespace MAuth.Web.Data.Repositories;

public class UserRepository(MAuthDbContext dbContext) : BaseRepository<User>(dbContext), IUserRepository
{
}

