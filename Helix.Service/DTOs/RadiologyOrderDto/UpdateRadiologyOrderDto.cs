using Helix.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace Helix.Service.DTOs.RadiologyOrderDto
{
    // 2. Used if a Doctor needs to edit an order before it is completed
    public class UpdateRadiologyOrderDto
    {
        [Required]
        public Guid Id { get; set; }
        public Guid? TerminologyCodeId { get; set; }
        public EnLabOrderStatus? Status { get; set; }
    }
}
