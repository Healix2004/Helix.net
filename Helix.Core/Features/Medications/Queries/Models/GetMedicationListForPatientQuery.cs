using Helix.Core.Bases;
using Helix.Service.DTOs.MedicationDTOs;
using MediatR;

namespace Helix.Core.Features.Medications.Queries.Models
{
    public class GetMedicationListForPatientQuery : IRequest<Response<IEnumerable<MedicationDto>>>
    {
        public Guid Id { get; set; }
        public GetMedicationListForPatientQuery(Guid id) => Id = id;
    }

}
