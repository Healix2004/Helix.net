using Helix.Core.Bases;
using MediatR;

namespace Helix.Core.Features.TerminologyCodeLookups.Commands.Models
{
    public class DeleteTerminologyCodeLookupCommand : IRequest<Response<bool>>
    {
        public Guid Id { get; set; }
        public DeleteTerminologyCodeLookupCommand(Guid id) => Id = id;
    }
}
