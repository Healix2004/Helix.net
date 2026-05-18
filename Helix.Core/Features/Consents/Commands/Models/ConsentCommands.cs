using Helix.Core.Bases;
using Helix.Service.DTOs.ConsentDTOs;
using MediatR;

namespace Helix.Core.Features.Consents.Commands.Models
{
    public class CreateConsentCommand : IRequest<Response<ConsentDto>>
    {
        public CreateConsentDto Dto { get; set; }
        public CreateConsentCommand(CreateConsentDto dto) => Dto = dto;
    }
}
