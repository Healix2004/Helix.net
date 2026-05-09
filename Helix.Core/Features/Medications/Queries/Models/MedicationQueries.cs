using Helix.Core.Bases;
using Helix.Service.DTOs.MedicationDTOs;
using MediatR;

namespace Helix.Core.Features.Medications.Queries.Models
{
    public class GetMedicationListQuery : IRequest<Response<IEnumerable<MedicationDto>>> { }

    public class GetMedicationByIdQuery : IRequest<Response<MedicationDto>>
    {
        public int Id { get; set; }
        public GetMedicationByIdQuery(int id) => Id = id;
    }
}
