using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public abstract class RepositoryBase<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext _context;

        public RepositoryBase(DbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<TEntity> GetByIdAsync(int id)
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _context.Set<TEntity>().ToListAsync();
        }

        public async Task AddAsync(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            _context.Set<TEntity>().Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Set<TEntity>().FindAsync(id);
            if (entity != null)
            {
                _context.Set<TEntity>().Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}