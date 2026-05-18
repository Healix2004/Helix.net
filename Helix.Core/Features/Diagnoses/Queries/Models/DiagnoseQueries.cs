using Helix.Core.Bases;
using Helix.Service.DTOs.DiagnoseDTOs;
using MediatR;

namespace Helix.Core.Features.Diagnoses.Queries.Models
{
    public class GetDiagnoseListQuery : IRequest<Response<IEnumerable<DiagnoseDto>>> { }

    public class GetDiagnoseByIdQuery : IRequest<Response<DiagnoseDto>>
    {
        public Guid Id { get; set; }
        public GetDiagnoseByIdQuery(Guid id) => Id = id;
    }
    public class GetDiagnoseListForPatientQuery : IRequest<Response<IEnumerable<DiagnoseDto>>>
    {
        public Guid PatiendId { get; set; }
        public GetDiagnoseListForPatientQuery(Guid patiendId) => this.PatiendId= patiendId;
    }
}
