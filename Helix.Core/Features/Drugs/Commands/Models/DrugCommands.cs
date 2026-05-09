using Helix.Core.Bases;
using Helix.Service.DTOs.DrugDTOs;
using MediatR;

namespace Helix.Core.Features.Drugs.Commands.Models
{
    public class CreateDrugCommand : IRequest<Response<DrugDTO>>
    {
        public CreateDrugDto Dto { get; set; }
        public CreateDrugCommand(CreateDrugDto dto) => Dto = dto;
    }

    public class UpdateDrugCommand : IRequest<Response<DrugDTO>>
    {
        public int Id { get; set; }
        public UpdateDrugDto Dto { get; set; }
        public UpdateDrugCommand(int id, UpdateDrugDto dto) { Id = id; Dto = dto; }
    }

    public class DeleteDrugCommand : IRequest<Response<bool>>
    {
        public int Id { get; set; }
        public DeleteDrugCommand(int id) => Id = id;
    }
}
