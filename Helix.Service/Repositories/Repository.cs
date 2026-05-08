using Helix.Data.Entities;
using Helix.Infrastructure.Context;
using Helix.Service.Interfaces;
using Helix.Service.Specification;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Helix.Service.Repositories
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly ApplicationDbContext context;

        public Repository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public void Add(T entity)
        {
            context.Set<T>().Add(entity);
          
        }

        public void Delete(T entity)
        {
            context.Set<T>().Remove(entity);
            
        }
        public void Update(T entity)
        {
            context.Set<T>().Update(entity);
           
        }

        public T Get(int Id) => context.Set<T>().Find(Id);
        public T Get(string Id) => context.Set<T>().Find(Id);

        public IEnumerable<T> GetALL() => context.Set<T>().AsNoTracking().ToList();

        public IQueryable<T> Find(Expression<Func<T, bool>> filter)
        {
           return context.Set<T>().Where(filter);
        }

        public T GetEntityWithSpec(ISpecification<T> spec) => ApplySpec(spec).FirstOrDefault();
        public IEnumerable<T> GetALLWithSpec(ISpecification<T> spec) => ApplySpec(spec).AsNoTracking().ToList();

        //helper
        private IQueryable<T> ApplySpec(ISpecification<T> spec) => SpecificationEvaluator<T>.GetQuery(context.Set<T>(), spec);       
    }
}
