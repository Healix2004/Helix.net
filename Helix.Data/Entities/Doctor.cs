using System;
using System.Collections.Generic;
using System.Text;

namespace Helix.Data.Entities
{
    public class Doctor : BaseEntity
    {
        // Relation With AppUser
        public string? UserId { get; set; }
        public virtual AppUser AppUser { get; set; }
        public string Specialization { get; set; } = default!;
         
    }
}
