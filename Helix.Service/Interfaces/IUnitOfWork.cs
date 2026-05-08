using Helix.Data.Entities;
using Helix.Service.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helix.Service.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        Repository<T> Repository<T>() where T : BaseEntity;

        int Complete();
    }
}
