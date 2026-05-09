using Helix.Core.Bases;
using Helix.Service.DTOs.ObservationDTOs;
using MediatR;

namespace Helix.Core.Features.Observations.Commands.Models
{
    public class CreateObservationCommand : IRequest<Response<ObservationDto>>
    {
        public CreateObservationDto Dto { get; set; }
        public CreateObservationCommand(CreateObservationDto dto) => Dto = dto;
    }

    public class UpdateObservationCommand : IRequest<Response<ObservationDto>>
    {
        public int Id { get; set; }
        public UpdateObservationDto Dto { get; set; }
        public UpdateObservationCommand(int id, UpdateObservationDto dto) { Id = id; Dto = dto; }
    }

    public class DeleteObservationCommand : IRequest<Response<bool>>
    {
        public int Id { get; set; }
        public DeleteObservationCommand(int id) => Id = id;
    }
}
