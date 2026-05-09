using Helix.Core.Bases;
using Helix.Service.DTOs.PatientDTOs;
using MediatR;

namespace Helix.Core.Features.Patients.Queries.Models
{
    public class GetPatientByIdQuery : IRequest<Response<PatientDto>>
    {
        public int Id { get; set; }
        public GetPatientByIdQuery(int id) => Id = id;
    }
}
