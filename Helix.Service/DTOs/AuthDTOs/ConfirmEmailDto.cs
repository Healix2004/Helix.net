using System.ComponentModel.DataAnnotations;

namespace Helix.Service.DTOs.AuthDTOs
{
    public class ConfirmEmailDto
    {
        [Required(ErrorMessage = "User Email is required")]
        public string UserEmail { get; set; }

        [Required(ErrorMessage = "Token is required")]
        public string code { get; set; }
    }
}

