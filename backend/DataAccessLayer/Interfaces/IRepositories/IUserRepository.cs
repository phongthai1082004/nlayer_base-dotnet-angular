using DataAccessLayer.Entities;
using DataAccessLayer.Interfaces.IRepositories.Common;

namespace DataAccessLayer.Interfaces.IRepositories
{
    public interface IUserRepository : IGenericRepository<User, Guid>
    {
    }
}
