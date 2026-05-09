using Helix.Core.Bases;
using Helix.Service.DTOs.EncounterDTOs;
using MediatR;

namespace Helix.Core.Features.Encounters.Commands.Models
{
    public class CreateEncounterCommand : IRequest<Response<EncounterDto>>
    {
        public CreateEncounterDto Dto { get; set; }
        public CreateEncounterCommand(CreateEncounterDto dto) => Dto = dto;
    }

    public class UpdateEncounterCommand : IRequest<Response<EncounterDto>>
    {
        public int Id { get; set; }
        public UpdateEncounterDto Dto { get; set; }
        public UpdateEncounterCommand(int id, UpdateEncounterDto dto) { Id = id; Dto = dto; }
    }

    public class DeleteEncounterCommand : IRequest<Response<bool>>
    {
        public int Id { get; set; }
        public DeleteEncounterCommand(int id) => Id = id;
    }
}
