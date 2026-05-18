using Helix.Core.Bases;
using Helix.Service.DTOs.ObservationDTOs;
using MediatR;

namespace Helix.Core.Features.Observations.Queries.Models
{
    public class GetObservationListQuery : IRequest<Response<IEnumerable<ObservationDto>>> { }

    public class GetObservationByIdQuery : IRequest<Response<ObservationDto>>
    {
        public Guid Id { get; set; }
        public GetObservationByIdQuery(Guid id) => Id = id;
    }
    
}
