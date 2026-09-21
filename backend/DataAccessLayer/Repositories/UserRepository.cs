using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using DataAccessLayer.Interfaces.IRepositories;
using DataAccessLayer.Repositories.Common;

namespace DataAccessLayer.Repositories
{
    public class UserRepository : GenericRepository<User, Guid>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context)
        {
        }
    }
}
