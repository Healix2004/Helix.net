using Helix.Core.Bases;
using Helix.Service.DTOs.DrugDTOs;
using MediatR;

namespace Helix.Core.Features.Drugs.Queries.Models
{
    public class GetDrugListQuery : IRequest<Response<IEnumerable<DrugDTO>>> { }
    public class CheckInteractionQuery : IRequest<Response<InteractionResponseDTO>>
    {
        public InteractionRequestDTO interactionRequestDTO { get; set; }
        public CheckInteractionQuery(InteractionRequestDTO interactionRequestDTO) => this.interactionRequestDTO = interactionRequestDTO;

    }
    public class GetDrugByIdQuery : IRequest<Response<DrugDTO>>
    {
        public int Id { get; set; }
        public GetDrugByIdQuery(int id) => Id = id;
    }
}
