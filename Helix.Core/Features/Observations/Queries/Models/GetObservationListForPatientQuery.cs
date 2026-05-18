using Helix.Core.Bases;
using Helix.Service.DTOs.ObservationDTOs;
using MediatR;

namespace Helix.Core.Features.Observations.Queries.Models
{
    public class GetObservationListForPatientQuery : IRequest<Response<IEnumerable<ObservationDto>>>
    {
        public Guid Id { get; set; }
        public GetObservationListForPatientQuery(Guid id) => Id = id;
    }
    
}
