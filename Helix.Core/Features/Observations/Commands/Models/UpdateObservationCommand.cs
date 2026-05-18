using Helix.Core.Bases;
using Helix.Service.DTOs.ObservationDTOs;
using MediatR;

namespace Helix.Core.Features.Observations.Commands.Models
{
    public class UpdateObservationCommand : IRequest<Response<ObservationDto>>
    {
        public Guid Id { get; set; }
        public UpdateObservationDto Dto { get; set; }
        public UpdateObservationCommand(Guid id, UpdateObservationDto dto) { Id = id; Dto = dto; }
    }
}
