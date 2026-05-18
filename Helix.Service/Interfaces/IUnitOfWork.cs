using Helix.Data.Entities;
using Helix.Service.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helix.Service.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<T> Repository<T>() where T : BaseEntity;
        int Complete();
        Task<int> CompleteAsync(CancellationToken cancellationToken = default);
    }
}
