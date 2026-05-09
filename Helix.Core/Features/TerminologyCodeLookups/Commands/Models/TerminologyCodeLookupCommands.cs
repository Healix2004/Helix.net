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

    public class UpdateTerminologyCodeLookupCommand : IRequest<Response<TerminologyCodeLookupDto>>
    {
        public int Id { get; set; }
        public UpdateTerminologyCodeLookupDto Dto { get; set; }
        public UpdateTerminologyCodeLookupCommand(int id, UpdateTerminologyCodeLookupDto dto) { Id = id; Dto = dto; }
    }

    public class DeleteTerminologyCodeLookupCommand : IRequest<Response<bool>>
    {
        public int Id { get; set; }
        public DeleteTerminologyCodeLookupCommand(int id) => Id = id;
    }
}
