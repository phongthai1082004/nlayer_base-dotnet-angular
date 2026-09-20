using DataAccessLayer.Data;
using DataAccessLayer.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories.Common
{
    public class BaseRepository<TEntity, TKey> : IBaseRepository<TEntity, TKey> where TEntity : class
    {
        private readonly AppDbContext _context;
        private DbSet<TEntity> _dbSet;
        public BaseRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }
        public async Task AddAsync(TEntity entity) => await _dbSet.AddAsync(entity);
        public async Task<IEnumerable<TEntity>> GetAllAsync() => await _dbSet.ToListAsync();
        public async Task<TEntity?> GetByIdAsync(TKey id) => await _dbSet.FindAsync();
        public void Remove(TEntity entity) => _dbSet.Remove(entity);
        public void Update(TEntity entity) => _dbSet.Update(entity);
    }
}
