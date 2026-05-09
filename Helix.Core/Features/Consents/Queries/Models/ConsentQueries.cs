using Helix.Core.Bases;
using Helix.Service.DTOs.ConsentDTOs;
using MediatR;

namespace Helix.Core.Features.Consents.Queries.Models
{
    public class GetConsentListQuery : IRequest<Response<IEnumerable<ConsentDto>>> { }

    public class GetConsentByIdQuery : IRequest<Response<ConsentDto>>
    {
        public int Id { get; set; }
        public GetConsentByIdQuery(int id) => Id = id;
    }
}
