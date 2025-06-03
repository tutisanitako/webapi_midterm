using FinalAPI.Domain.Interfaces;
using FinalAPI.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace FinalAPI.Infrastructure.Repositories
{
    public abstract class RepositoryBase<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly HealthDbContext _context;

        public RepositoryBase(HealthDbContext context)
        {
            _context = context;
        }

        public Task<TEntity> GetByIdAsync(int id)
        {
            return Task.FromResult(_context.Set<TEntity>().Find(id));
        }

        public Task<IEnumerable<TEntity>> GetAllAsync()
        {
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
            var entity = _context.Set<TEntity>().Find(id);
            if (entity != null)
            {
                _context.Set<TEntity>().Remove(entity);
                _context.SaveChanges();
            }
            return Task.CompletedTask;
        }
    }
}