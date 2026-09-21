using DataAccessLayer.Data;
using DataAccessLayer.Interfaces.IRepositories.Common;

namespace DataAccessLayer.Repositories.Common
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }
        public IGenericRepository<TEntity, TKey> GenerateRepository<TEntity, TKey>() where TEntity : class
        {
            return new GenericRepository<TEntity, TKey>(_context);
        }
        public async Task<int> SaveChangesAsync(CancellationToken ct = default) => await _context.SaveChangesAsync(ct);
    }
}
