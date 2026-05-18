
using Helix.Data.Entities;
using Helix.Infrastructure.Context;
using Helix.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helix.Service.Repositories
{
    public class UnitOfWork(ApplicationDbContext dbContext) : IUnitOfWork, IAsyncDisposable
    {
        private readonly ConcurrentDictionary<Type, object> _repositories = new();
        public IRepository<T> Repository<T>() where T : BaseEntity
        {
            var type = typeof(T);
            return (IRepository<T>)_repositories.GetOrAdd(type, _ => new Repository<T>(dbContext));
        }
        public int Complete() => dbContext.SaveChanges();
        public async Task<int> CompleteAsync(CancellationToken cancellationToken = default)=>await dbContext.SaveChangesAsync(cancellationToken);
        public void Dispose()=> dbContext.Dispose();
        public async ValueTask DisposeAsync()=>await dbContext.DisposeAsync();
    }
}
