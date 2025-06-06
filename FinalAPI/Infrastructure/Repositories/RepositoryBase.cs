using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity; // Using System.Data.Entity for EF6
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public abstract class RepositoryBase<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext _context; // Changed to System.Data.Entity.DbContext

        public RepositoryBase(DbContext context)
        {
            _context = context;
        }

        // Changed return type to Task<TEntity> and handle null explicitly
        public Task<TEntity> GetByIdAsync(int id)
        {
            // For EF6 synchronous Find, wrap in Task.FromResult
            return Task.FromResult(_context.Set<TEntity>().Find(id));
        }

        // Changed return type to Task<IEnumerable<TEntity>> to match IRepository
        public Task<IEnumerable<TEntity>> GetAllAsync()
        {
            // For EF6 synchronous AsEnumerable, wrap in Task.FromResult
            return Task.FromResult(_context.Set<TEntity>().AsEnumerable());
        }

        public Task AddAsync(TEntity entity)
        {
            _context.Set<TEntity>().Add(entity);
            _context.SaveChanges();
            return Task.CompletedTask;
        }

        public Task UpdateAsync(TEntity entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();
            return Task.CompletedTask;
        }

        public Task DeleteAsync(int id)
        {
            // Call the local GetByIdAsync for consistency in null check
            var entity = _context.Set<TEntity>().Find(id); // Synchronous find for EF6
            if (entity != null)
            {
                _context.Set<TEntity>().Remove(entity);
                _context.SaveChanges();
            }
            return Task.CompletedTask;
        }
    }
}
