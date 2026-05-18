using Helix.Core.Bases;
using Helix.Service.DTOs.AllergyDTOs;
using MediatR;

namespace Helix.Core.Features.Allergies.Commands.Models
{
    public class UpdateAllergyCommand : IRequest<Response<AllergyDto>>
    {
        public Guid Id { get; set; }
        public UpdateAllergyDto Dto { get; set; }
        public UpdateAllergyCommand(Guid id, UpdateAllergyDto dto) { Id = id; Dto = dto; }
    }
}
