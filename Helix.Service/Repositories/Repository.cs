using Helix.Data.Entities;
using Helix.Infrastructure.Context;
using Helix.Service.Interfaces;
using Helix.Service.Specification;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Helix.Service.Repositories
{
    public class Repository<T>(ApplicationDbContext context) : IRepository<T> where T : BaseEntity
    {
        private readonly DbSet<T> _dbSet = context.Set<T>();
        // 1. Core CRUD
        public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
            return entity;
        }
        public Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            _dbSet.Update(entity);
            return Task.CompletedTask;
        }
        public Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            _dbSet.Remove(entity);
            return Task.CompletedTask;
        }
        // 2. Retrieval by ID
        public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FindAsync([id], cancellationToken);
        }
        public async Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FindAsync([id], cancellationToken);
        }

        // 3. Collections
        public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet.ToListAsync(cancellationToken);
        }

        // 4. Filtering
        public async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default)
        {
            return await _dbSet.Where(filter).ToListAsync(cancellationToken);
        }

        public async Task<IQueryable<T>> FindAsQueryable(Expression<Func<T, bool>> filter)
        {
            // Returns the queryable without executing it yet
            return await Task.FromResult(_dbSet.Where(filter));
        }

        // 5. Specifications
        public async Task<T?> GetEntityWithSpecAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
        {
            return await ApplySpecification(spec).FirstOrDefaultAsync(cancellationToken);
        }
        public async Task<IQueryable<T>> GetAllWithSpecAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(ApplySpecification(spec));
        }
        // Helper Method to use the SpecificationEvaluator
        private IQueryable<T> ApplySpecification(ISpecification<T> spec)
        {
            return SpecificationEvaluator<T>.GetQuery(_dbSet.AsQueryable(), spec);
        }
    }

}
