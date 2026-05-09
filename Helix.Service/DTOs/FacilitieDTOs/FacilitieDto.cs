using Helix.Data.Enums;
using System;

namespace Helix.Service.DTOs.FacilitieDTOs
{
    public class FacilitieDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string SubscriptionPlan { get; set; }
        public EnFacilitieType FacilitieType { get; set; }
    }
}
