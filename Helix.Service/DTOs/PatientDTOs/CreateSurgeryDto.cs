using System.ComponentModel.DataAnnotations;

namespace Helix.Service.DTOs.PatientDTOs
{
    public class CreateSurgeryDto
    {
        [Required(ErrorMessage = "Procedure code is required")]
        [MaxLength(50)]
        public string ProcedureCatalogCode { get; set; } 
    }
}