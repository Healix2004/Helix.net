using Helix.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Helix.Data.Entities
{
    public class Facilitie : BaseEntity
    {//Name, Type (Clinic, Hospital), Address, SubscriptionPlan (e.g., 90K, 180K, 250K
        public string Name { get; set; }
        public string Address { get; set; }
        public string SubscriptionPlan { get; set; }
        public EnFacilitieType FacilitieType { get; set; }

        // many to many doctors
        public List<Doctor> Doctors { get; set; }
    }
}
