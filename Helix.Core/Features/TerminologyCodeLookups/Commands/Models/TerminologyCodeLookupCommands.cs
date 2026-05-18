using Helix.Core.Bases;
using Helix.Service.DTOs.TerminologyCodeLookupDTOs;
using MediatR;

namespace Helix.Core.Features.TerminologyCodeLookups.Commands.Models
{
    public class CreateTerminologyCodeLookupCommand : IRequest<Response<TerminologyCodeLookupDto>>
    {
        public CreateTerminologyCodeLookupDto Dto { get; set; }
        public CreateTerminologyCodeLookupCommand(CreateTerminologyCodeLookupDto dto) => Dto = dto;
    }
}
