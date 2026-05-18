using Helix.Core.Bases;
using Helix.Service.DTOs.MedicationDTOs;
using MediatR;

namespace Helix.Core.Features.Medications.Queries.Models
{
    public class GetMedicationListQuery : IRequest<Response<IEnumerable<MedicationDto>>> { }
}
