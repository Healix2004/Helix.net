using System;
using System.Collections.Generic;
using System.Text;

namespace Helix.Data.Entities
{
    public class BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
    }
}
