using Helix.Core.Bases;
using Helix.Service.DTOs.ConsentDTOs;
using MediatR;

namespace Helix.Core.Features.Consents.Commands.Models
{
    public class UpdateConsentCommand : IRequest<Response<ConsentDto>>
    {
        public Guid Id { get; set; }
        public UpdateConsentDto Dto { get; set; }
        public UpdateConsentCommand(Guid id, UpdateConsentDto dto) { Id = id; Dto = dto; }
    }
}
