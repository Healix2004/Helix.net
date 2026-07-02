namespace Helix.Service.DTOs.DrugDTOs
{
    public class CreateDrugDto
    {
        public string Name { get; set; }
    }
    public class UpdateDrugDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
    public class PrescriptionSafetyCheckDto
    {
        public Guid PatientId { get; set; }
        public List<string> NewMedicationIds { get; set; } = new();
    }

    public class PrescriptionSafetyResultDto
    {
        public bool IsSafe { get; set; }
        public List<InteractionResponseDTO> DangerousInteractions { get; set; } = new();
    }
}
