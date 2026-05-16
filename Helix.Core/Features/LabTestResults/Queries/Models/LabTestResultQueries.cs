using Helix.Core.Bases;
using Helix.Service.DTOs.LabTestResultDTOs;
using Hl7.Fhir.Model;
using MediatR;

namespace Helix.Core.Features.LabTestResults.Queries.Models
{
    public class GetLabTestResultListQuery : IRequest<Response<IEnumerable<LabTestResultDto>>> { }

    public class GetLabTestResultListForPatientQuery : IRequest<Response<IEnumerable<LabTestResultDto>>> {
        public Guid PatientId { get; set; }
        public GetLabTestResultListForPatientQuery(Guid PatientId)=>this.PatientId = PatientId;
    }

    public class GetLabTestResultByIdQuery : IRequest<Response<LabTestResultDto>>
    {
        public int Id { get; set; }
        public GetLabTestResultByIdQuery(int id) => Id = id;
    }
}
