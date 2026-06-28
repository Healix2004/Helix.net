using Helix.Data.Enums;

namespace Helix.Data.Entities
{
    public class Appointment : BaseEntity
    {
        // --- Core FHIR Data ---
        public EnAppointmentStatus Status { get; set; } = EnAppointmentStatus.Pending;
        public EnAppointmentPriority Priority { get; set; } = EnAppointmentPriority.Routine;

        // FHIR uses start and end times rather than just a "time" string
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        // e.g., "Cardiology Checkup", "Follow-up Consultation"
        // In a strict FHIR system, this links to a MedicalConcept/Terminology catalog
        public string AppointmentType { get; set; } = string.Empty;

        public string? PatientInstruction { get; set; } // e.g., "Fast for 12 hours before"

        // --- Foreign Keys ---
        public Guid PatientId { get; set; }

        // In FHIR, the doctor is an "Actor" participating in the appointment.
        public Guid DoctorId { get; set; }

        // --- Navigation Properties ---
        public virtual Patient Patient { get; set; } = null!;
        public virtual Doctor Doctor { get; set; } = null!;
    }
}