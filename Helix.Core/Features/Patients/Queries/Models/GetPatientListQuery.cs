using Helix.Core.Bases;
using Helix.Service.DTOs.PatientDTOs;
using MediatR;

namespace Helix.Core.Features.Patients.Queries.Models
{
    public class GetPatientListQuery : IRequest<Response<IEnumerable<PatientDto>>>
    {
    }
}
