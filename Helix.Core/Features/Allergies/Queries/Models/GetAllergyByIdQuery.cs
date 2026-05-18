using Helix.Core.Bases;
using Helix.Service.DTOs.AllergyDTOs;
using MediatR;

namespace Helix.Core.Features.Allergies.Queries.Models
{
    public class GetAllergyByIdQuery : IRequest<Response<AllergyDto>>
    {
        public Guid Id { get; set; }
        public GetAllergyByIdQuery(Guid id) => Id = id;

    }
}
