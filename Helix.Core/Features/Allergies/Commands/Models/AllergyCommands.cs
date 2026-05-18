using Helix.Core.Bases;
using Helix.Service.DTOs.AllergyDTOs;
using MediatR;

namespace Helix.Core.Features.Allergies.Commands.Models
{
    public class CreateAllergyCommand : IRequest<Response<AllergyDto>>
    {
        public CreateAllergyDto Dto { get; set; }
        public CreateAllergyCommand(CreateAllergyDto dto) => Dto = dto;
    }
}
