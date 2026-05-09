using Helix.Core.Bases;
using Helix.Service.DTOs.FacilitieDTOs;
using MediatR;

namespace Helix.Core.Features.Facilities.Commands.Models
{
    public class CreateFacilityCommand : IRequest<Response<FacilitieDto>>
    {
        public CreateFacilitieDto Dto { get; set; }
        public CreateFacilityCommand(CreateFacilitieDto dto) => Dto = dto;
    }

    public class UpdateFacilityCommand : IRequest<Response<FacilitieDto>>
    {
        public int Id { get; set; }
        public UpdateFacilitieDto Dto { get; set; }
        public UpdateFacilityCommand(int id, UpdateFacilitieDto dto) { Id = id; Dto = dto; }
    }

    public class DeleteFacilityCommand : IRequest<Response<bool>>
    {
        public int Id { get; set; }
        public DeleteFacilityCommand(int id) => Id = id;
    }
}
