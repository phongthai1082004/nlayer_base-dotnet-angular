namespace DataAccessLayer.Interfaces.IRepositories.Common
{
    public interface IUnitOfWork
    {
        IGenericRepository<TEntity, TKey> GenerateRepository<TEntity, TKey>() where TEntity : class;
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}
