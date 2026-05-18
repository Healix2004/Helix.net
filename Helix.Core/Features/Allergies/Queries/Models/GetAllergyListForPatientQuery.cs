using Helix.Core.Bases;
using Helix.Service.DTOs.AllergyDTOs;
using MediatR;

namespace Helix.Core.Features.Allergies.Queries.Models
{
    public class GetAllergyListForPatientQuery : IRequest<Response<IEnumerable<AllergyDto>>> 
    { 
        public Guid Id { get; set; }
        public GetAllergyListForPatientQuery(Guid id) => Id = id;
    }
}
