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
        public void Add(T entity);

        public void Update(T entity);

        public void Delete(T entity);

        public T Get(int Id);
        public T Get(string Id);

        public IEnumerable<T> GetALL();

        public IQueryable<T> Find(Expression<Func<T, bool>> filter);
       

        public T GetEntityWithSpec(ISpecification<T> spec); 
        public IEnumerable<T> GetALLWithSpec(ISpecification<T> spec);
    }
}
