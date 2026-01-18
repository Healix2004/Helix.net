using System.ComponentModel.DataAnnotations;

namespace Helix.Service.DTOs.AuthDTOs
{
    public class ConfirmEmailDto
    {
        [Required(ErrorMessage = "User ID is required")]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "Token is required")]
        public string Token { get; set; }
    }
}

