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
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly ApplicationDbContext context;

        public Repository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public Task Add(T entity)
        {
            context.Set<T>().Add(entity);
            return Task.CompletedTask;
        }

        public Task Delete(T entity)
        {
            context.Set<T>().Remove(entity);
            return Task.CompletedTask;
        }

        public Task Update(T entity)
        {
            context.Set<T>().Update(entity);
            return Task.CompletedTask;
        }

        public Task<T> Get(Guid Id) => Task.FromResult(context.Set<T>().Find(Id));
        public Task<T> Get(string Id) => Task.FromResult(context.Set<T>().Find(Id));

         public Task<IEnumerable<T>> GetALL()
        {
            // ensure the returned Task<T> generic matches IEnumerable<T>
            return Task.FromResult<IEnumerable<T>>(context.Set<T>().AsNoTracking().ToList());
        }

        public Task<IQueryable<T>> Find(Expression<Func<T, bool>> filter)
        {
            return Task.FromResult(context.Set<T>().Where(filter));
        }

        public Task<T> GetEntityWithSpec(ISpecification<T> spec) => Task.FromResult(ApplySpec(spec).FirstOrDefault());
        public Task<IEnumerable<T>> GetALLWithSpec(ISpecification<T> spec) => Task.FromResult<IEnumerable<T>>(ApplySpec(spec).AsNoTracking().ToList());

        //helper
        private IQueryable<T> ApplySpec(ISpecification<T> spec) => SpecificationEvaluator<T>.GetQuery(context.Set<T>(), spec);
    }
}
