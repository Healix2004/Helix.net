using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helix.Infrastructure.Context.DbInitializer
{
    public interface IDbInitializer
    {
        public Task Initialize();
    }
}
