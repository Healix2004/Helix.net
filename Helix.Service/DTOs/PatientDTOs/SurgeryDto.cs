namespace Helix.Service.DTOs.PatientDTOs
{
    // NEW: Returns the detailed FHIR procedure information
    public class SurgeryDto
    {
        public Guid Id { get; set; }
        public string ProcedureCatalogCode { get; set; } // The SNOMED Code
        public string ProcedureName { get; set; }        // The Display Name (Flattened from Catalog)
        public DateTime DateOfSurgery { get; set; }
        public string SurgeonName { get; set; }
        public string HospitalOrClinicName { get; set; }
        public string MedicalNotes { get; set; }
    }
}