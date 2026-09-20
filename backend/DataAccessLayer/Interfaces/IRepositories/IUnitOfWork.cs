using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLayer.Interfaces.IRepositories
{
    public interface IUnitOfWork : IDisposable
    {
        IBaseRepository<TEntity, TKey> GenerateRepository<TEntity, TKey>() where TEntity : class;
        Task<int> SaveChangesAsync();
    }
}
