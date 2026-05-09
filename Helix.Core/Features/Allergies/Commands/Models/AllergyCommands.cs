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

    public class UpdateAllergyCommand : IRequest<Response<AllergyDto>>
    {
        public int Id { get; set; }
        public UpdateAllergyDto Dto { get; set; }
        public UpdateAllergyCommand(int id, UpdateAllergyDto dto) { Id = id; Dto = dto; }
    }

    public class DeleteAllergyCommand : IRequest<Response<bool>>
    {
        public int Id { get; set; }
        public DeleteAllergyCommand(int id) => Id = id;
    }
}
