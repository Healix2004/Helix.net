using Helix.Core.Bases;
using Helix.Service.DTOs.PatientDTOs;
using MediatR;

namespace Helix.Core.Features.Patients.Queries.Models
{
    public class GetPatientByIdQuery : IRequest<Response<PatientDto>>
    {
        public Guid Id { get; set; }
        public GetPatientByIdQuery(Guid id) => Id = id;
    }
}
