using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmokingCessation.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        // Generic repository
        IGenericRepository<TEntity, TKey> Repository<TEntity, TKey>() where TEntity : class;

        // Use for simple inserts, updates, or deletions

        int SaveChanges();
        Task<int> SaveChangesAsync();

        // Use for operations when requiring data consistency accross multiple tables
        // or when handling complex logic with dependencies
        int SaveChangesWithTransaction();
        Task<int> SaveChangesWithTransactionAsync();
        Task BeginTransactionAsync();

        // Commit the transaction
        Task CommitAsync();

        // Rollback the transaction
        Task RollbackAsync();
    }
}

