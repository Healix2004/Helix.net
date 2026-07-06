using Helix.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Helix.Data.Entities
{
    // The "Parent" document created during the visit
    public class Prescription : BaseEntity
    {
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public Guid? AppointmentId { get; set; }
        public string? DoctorNotes { get; set; }
        public virtual Patient Patient { get; set; } = null!;
        public virtual Doctor Doctor { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public ICollection<LabOrder> LabOrders { get; set; } = new List<LabOrder>();
        public ICollection<RadiologyOrder> RadiologyOrders { get; set; } = new List<RadiologyOrder>();
        public virtual List<PrescriptionItem> Items { get; set; } = new List<PrescriptionItem>();
    }
    public class PrescriptionItem : BaseEntity
    {
        public Guid PrescriptionId { get; set; }
        public string MedicationCatalogRxcui { get; set; } // Linked to your catalog

        public string Dosage { get; set; } = string.Empty;
        public string Frequency { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public EnPrescriptionItemStatus Status { get; set; } = EnPrescriptionItemStatus.Active;
        public virtual Prescription Prescription { get; set; } = null!;
        public virtual MedicationCatalog MedicationCatalog { get; set; } = null!;
    }
}
