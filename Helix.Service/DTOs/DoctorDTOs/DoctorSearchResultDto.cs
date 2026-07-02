namespace Helix.Service.DTOs.DoctorDTOs
{
    public class DoctorSearchResultDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Specialty { get; set; } = string.Empty;
        public string ProfilePhotoUrl { get; set; } = string.Empty;
    }
}
