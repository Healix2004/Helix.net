
using Helix.Data.Entities;
using Helix.Infrastructure.Context;
using Helix.Service.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helix.Service.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext dbcontext;

        private Hashtable _repsitories;
        public UnitOfWork(ApplicationDbContext dbcontext)
        {
            this.dbcontext = dbcontext;
            _repsitories = new Hashtable();


        }
        public Repository<T> Repository<T>() where T : BaseEntity
        {
            var Key = typeof(T).Name;
            if (!_repsitories.ContainsKey(Key))
            {
                var repo = new Repository<T>(dbcontext);
                _repsitories.Add(Key, repo);
            }
            return _repsitories[Key] as Repository<T>;
        }


        public int Complete()
        {
           return dbcontext.SaveChanges();
            
        }

        public void Dispose()
        {
            dbcontext.Dispose();
        }

    
    }
}
