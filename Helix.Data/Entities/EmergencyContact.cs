namespace Helix.Data.Entities
{
    public class EmergencyContact : BaseEntity
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }

        // Foreign Key
        public Guid PatientId { get; set; }
        public virtual Patient Patient { get; set; }
    }
}