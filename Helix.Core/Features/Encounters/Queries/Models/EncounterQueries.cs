using Helix.Core.Bases;
using Helix.Service.DTOs.EncounterDTOs;
using MediatR;

namespace Helix.Core.Features.Encounters.Queries.Models
{
    public class GetEncounterListQuery : IRequest<Response<IEnumerable<EncounterDto>>> { }

    public class GetEncounterByIdQuery : IRequest<Response<EncounterDto>>
    {
        public int Id { get; set; }
        public GetEncounterByIdQuery(int id) => Id = id;
    }
}
