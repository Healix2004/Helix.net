using Helix.Data.Enums;

namespace Helix.Data.Entities
{
    public class RadiologyOrder : BaseEntity
    {
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public Guid TerminologyCodeId { get; set; } // The specific scan requested (e.g., Code 70551 for "MRI Brain")
        public string QrToken { get; set; } = string.Empty;
        public EnLabOrderStatus Status { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;

        // 3. Navigation Properties (Linking to the rest of HELIX)
        public virtual Patient? Patient { get; set; }
        public virtual Doctor? Doctor { get; set; }
        public virtual TerminologyCodeLookup? TerminologyCode { get; set; }
        public virtual RadiologyResult? Result { get; set; }
    }
}
