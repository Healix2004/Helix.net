using Helix.Core.Bases;
using Helix.Service.DTOs.DiagnoseDTOs;
using MediatR;

namespace Helix.Core.Features.Diagnoses.Queries.Models
{
    public class GetDiagnoseListQuery : IRequest<Response<IEnumerable<DiagnoseDto>>> { }

    public class GetDiagnoseByIdQuery : IRequest<Response<DiagnoseDto>>
    {
        public int Id { get; set; }
        public GetDiagnoseByIdQuery(int id) => Id = id;
    }
}
