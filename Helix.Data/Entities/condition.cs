using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Helix.Data.Entities
{
    public class condition : BaseEntity
    {
        public string BodySite { get; set; }
        public string summary { get; set; }

        public bool cured { get; set; }

        //Encounter relationship 
        public Guid EncounterId { get; set; }   
        public Encounter Encounter {get;set;}
    }
}
