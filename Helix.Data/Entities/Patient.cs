using Helix.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Helix.Data.Entities
{
    public class Patient : BaseEntity
    {
        // Relation With AppUser
        public virtual AppUser AppUser { get; set; }
        public  EnPatientCategories PatientCategory { get; set; }
    }
}
