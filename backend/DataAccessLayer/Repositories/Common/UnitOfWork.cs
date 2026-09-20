using DataAccessLayer.Data;
using DataAccessLayer.Interfaces.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLayer.Repositories.Common
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }
        public IBaseRepository<TEntity, TKey> GenerateRepository<TEntity, TKey>() where TEntity : class
        {
            return new BaseRepository<TEntity, TKey>(_context);
        }
        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
        public void Dispose() => _context.Dispose();
    }
}
