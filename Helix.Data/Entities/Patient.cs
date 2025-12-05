using Helix.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Helix.Data.Entities
{
    public class Patient : AppUser
    {
        public  EnPatientCategories PatientCategory { get; set; }
    }
}
