namespace Helix.Service.DTOs.PatientDTOs
{
    // A small DTO to represent the EmergencyContact entity data being sent to the UI
    public class EmergencyContactDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
    }
}