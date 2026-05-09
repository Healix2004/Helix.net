using Helix.Data.Enums;

namespace Helix.Service.DTOs.FacilitieDTOs
{
    public class CreateFacilitieDto
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string SubscriptionPlan { get; set; }
        public EnFacilitieType FacilitieType { get; set; }
    }
}
