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
}
