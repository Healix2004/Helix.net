using Helix.Core.Bases;
using Helix.Service.DTOs.TerminologyCodeLookupDTOs;
using MediatR;

namespace Helix.Core.Features.TerminologyCodeLookups.Commands.Models
{
    public class UpdateTerminologyCodeLookupCommand : IRequest<Response<TerminologyCodeLookupDto>>
    {
        public Guid Id { get; set; }
        public UpdateTerminologyCodeLookupDto Dto { get; set; }
        public UpdateTerminologyCodeLookupCommand(Guid id, UpdateTerminologyCodeLookupDto dto) { Id = id; Dto = dto; }
    }
}
