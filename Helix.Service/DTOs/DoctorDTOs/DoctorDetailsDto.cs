namespace Helix.Service.DTOs.DoctorDTOs
{
    public class DoctorDetailsDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Specialty { get; set; } = string.Empty;
        public string ClinicAddress { get; set; } = string.Empty;
        public decimal ConsultationFees { get; set; }
        public int YearsOfExperience { get; set; }
        public string ProfilePhotoUrl { get; set; } = string.Empty;
    }
}
