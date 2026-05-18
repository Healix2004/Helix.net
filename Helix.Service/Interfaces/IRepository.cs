using Helix.Data.Entities;
using Helix.Service.Specification;
using System.Linq.Expressions;

namespace Helix.Service.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        // 1. Core CRUD
        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(T entity, CancellationToken cancellationToken = default);

        // 2. Retrieval by ID
        Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default);

        // 3. Collections
        Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);

        // 4. Filtering
        Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default);
        Task<IQueryable<T>> FindAsQueryable(Expression<Func<T, bool>> filter);

        // 5. Specifications
        Task<T?> GetEntityWithSpecAsync(ISpecification<T> spec, CancellationToken cancellationToken = default);
        Task<IQueryable<T>> GetAllWithSpecAsync(ISpecification<T> spec, CancellationToken cancellationToken = default);
    }
}
