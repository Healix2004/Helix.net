using Helix.Core.Bases;
using Helix.Service.DTOs.TerminologyCodeLookupDTOs;
using MediatR;

namespace Helix.Core.Features.TerminologyCodeLookups.Queries.Models
{
    public class GetTerminologyCodeLookupListQuery : IRequest<Response<IEnumerable<TerminologyCodeLookupDto>>> { }

    public class GetTerminologyCodeLookupByIdQuery : IRequest<Response<TerminologyCodeLookupDto>>
    {
        public int Id { get; set; }
        public GetTerminologyCodeLookupByIdQuery(int id) => Id = id;
    }
}
