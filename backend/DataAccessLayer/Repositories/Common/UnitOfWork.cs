using DataAccessLayer.Data;
using DataAccessLayer.Interfaces.IRepositories.Common;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace DataAccessLayer.Repositories.Common
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction? _currentTransaction;
        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }
        public IGenericRepository<TEntity, TKey> GenerateRepository<TEntity, TKey>() where TEntity : class
        {
            return new GenericRepository<TEntity, TKey>(_context);
        }
        public async Task<int> SaveChangesAsync(CancellationToken ct = default) => await _context.SaveChangesAsync(ct);
    
        public async Task BeginTransactionAsync(CancellationToken ct = default)
        {
            if (_currentTransaction != null)
            {
                throw new InvalidOperationException("A transaction is already in progress.");
            }
            _currentTransaction = await _context.Database.BeginTransactionAsync(ct);
        }

        public async Task CommitTransactionAsync(CancellationToken ct = default)
        {
            try
            {
                if (_currentTransaction == null)
                    throw new InvalidOperationException("No transaction is in progress.");
                await _currentTransaction.CommitAsync(ct);
            }
            catch
            {
                if (_currentTransaction != null)
                    await _currentTransaction.RollbackAsync(ct);
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    await _currentTransaction.DisposeAsync();
                    _currentTransaction = null;
                }
            }
        }
        public async Task RollbackTransactionAsync(CancellationToken ct = default)
        {
            try
            {
                if (_currentTransaction != null)
                {
                    await _currentTransaction.RollbackAsync(ct);
                }
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public void TransactionDispose()
        {
            _currentTransaction?.Dispose();
            _context.Dispose();
        }
    }
}
