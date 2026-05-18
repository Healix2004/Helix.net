using Helix.Core.Bases;
using Helix.Service.DTOs.MedicationDTOs;
using MediatR;

namespace Helix.Core.Features.Medications.Queries.Models
{
    public class GetMedicationByIdQuery : IRequest<Response<MedicationDto>>
    {
        public Guid Id { get; set; }
        public GetMedicationByIdQuery(Guid id) => Id = id;
    }

}
