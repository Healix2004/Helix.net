using Helix.Core.Bases;
using Helix.Service.DTOs.AllergyDTOs;
using MediatR;

namespace Helix.Core.Features.Allergies.Queries.Models
{
    public class GetAllergyListQuery : IRequest<Response<IEnumerable<AllergyDto>>> { }
}
