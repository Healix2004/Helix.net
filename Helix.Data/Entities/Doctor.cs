using System;
using System.Collections.Generic;
using System.Text;

namespace Helix.Data.Entities
{
    public class Doctor : AppUser
    {
        public string Specialization { get; set; } = default!;
         
    }
}
