using Helix.Data.Entities;
using Helix.Service.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Helix.Service.Interfaces
{
    public interface IRepository <T> where T : BaseEntity
    {
        public Task Add(T entity);

        public Task Update(T entity);

        public Task Delete(T entity);

        public Task<T> Get(Guid Id);
        public Task<T> Get(string Id);

        public Task<IEnumerable<T>> GetALL();

        public Task<IQueryable<T>> Find(Expression<Func<T, bool>> filter);
       

        public Task<T> GetEntityWithSpec(ISpecification<T> spec); 
        public Task<IEnumerable<T>> GetALLWithSpec(ISpecification<T> spec);
    }
}
