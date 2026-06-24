using Helix.Data.Enums;

namespace Helix.Service.DTOs.AuthDTOs
{
    /// <summary>
    /// DTO for user profile information (safe for returning to clients)
    /// </summary>
    public class UserDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string? AlternativeEmailAddress { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
    }
}

