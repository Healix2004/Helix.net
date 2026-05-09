using Helix.Core.Bases;
using Helix.Service.DTOs.FacilitieDTOs;
using MediatR;

namespace Helix.Core.Features.Facilities.Queries.Models
{
    public class GetFacilityListQuery : IRequest<Response<IEnumerable<FacilitieDto>>> { }

    public class GetFacilityByIdQuery : IRequest<Response<FacilitieDto>>
    {
        public int Id { get; set; }
        public GetFacilityByIdQuery(int id) => Id = id;
    }
}
