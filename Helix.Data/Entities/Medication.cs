using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Helix.Data.Entities
{
    public class Medication:BaseEntity
    {
        public Guid PatientId { get; set; }
        public string medicationCatalogRxcui { get; set; }
        public MedicationCatalog medicationCatalog { get; set; }
        public string Dosage {  get; set; } = string.Empty;
        public string Frequency { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; } = new DateOnly(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day);
        public DateOnly? EndDate { get; set; }
    }
}
