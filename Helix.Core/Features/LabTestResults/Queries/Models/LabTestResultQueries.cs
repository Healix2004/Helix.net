using Helix.Core.Bases;
using Helix.Service.DTOs.LabTestResultDTOs;
using MediatR;

namespace Helix.Core.Features.LabTestResults.Queries.Models
{
    public class GetLabTestResultListQuery : IRequest<Response<IEnumerable<LabTestResultDto>>> { }

    public class GetLabTestResultByIdQuery : IRequest<Response<LabTestResultDto>>
    {
        public int Id { get; set; }
        public GetLabTestResultByIdQuery(int id) => Id = id;
    }
}
