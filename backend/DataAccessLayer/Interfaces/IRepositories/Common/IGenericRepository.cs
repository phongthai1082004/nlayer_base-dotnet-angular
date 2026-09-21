using System.Linq.Expressions;

namespace DataAccessLayer.Interfaces.IRepositories.Common
{
    public interface IGenericRepository<TEntity, TKey> where TEntity : class
    {
        Task<TEntity?> GetByIdAsync(TKey id, CancellationToken ct = default);
        Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken ct = default);
        Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> expression, CancellationToken ct = default);
        Task<bool> AnyWhereAsync(Expression<Func<TEntity, bool>> expression, CancellationToken ct = default);
        Task<TEntity> AddAsync(TEntity entity, CancellationToken ct = default);
        void AddRange(IEnumerable<TEntity> entities);
        void Update(TEntity entity); 
        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);
    }
}
