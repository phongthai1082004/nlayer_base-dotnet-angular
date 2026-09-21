using DataAccessLayer.Data;
using DataAccessLayer.Interfaces.IRepositories.Common;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DataAccessLayer.Repositories.Common
{
    public class GenericRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey> where TEntity : class
    {
        protected readonly AppDbContext _context;
        protected DbSet<TEntity> _dbSet;
        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }
        public async Task<TEntity?> GetByIdAsync(TKey id, CancellationToken ct = default) => await _dbSet.FindAsync(id, ct);
        public async Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> expression, CancellationToken ct = default) => await _dbSet.Where(expression).FirstOrDefaultAsync(ct);
        public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken ct = default) => await _dbSet.ToListAsync(ct);
        public async Task<bool> AnyWhereAsync(Expression<Func<TEntity, bool>> expression, CancellationToken ct = default) => await _dbSet.AnyAsync(expression, ct);
        public async Task<TEntity> AddAsync(TEntity entity, CancellationToken ct = default) => (await _dbSet.AddAsync(entity, ct)).Entity;
        public void AddRange(IEnumerable<TEntity> entities) => _dbSet.AddRange(entities);
        public void Update(TEntity entity) => _dbSet.Update(entity);
        public void Remove(TEntity entity) => _dbSet.Remove(entity);
        public void RemoveRange(IEnumerable<TEntity> entities) => _dbSet.RemoveRange(entities);
    }
}
