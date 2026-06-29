using Helix.Core.Bases;
using Helix.Core.Features.Drugs.Queries.Models;
using Helix.Service.DTOs.DrugDTOs;
using Helix.Service.Interfaces;
using MediatR;

namespace Helix.Core.Features.Drugs.Queries.Handler
{
    public class GetDrugListQueryHandler : IRequestHandler<GetDrugListQuery, Response<IEnumerable<DrugDTO>>>
    {
        private readonly IDrugDataService _drugDataService;
        private readonly ResponseHandler _responseHandler;

        public GetDrugListQueryHandler(IDrugDataService drugDataService, ResponseHandler responseHandler)
        {
            _drugDataService = drugDataService;
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public async Task<Response<IEnumerable<DrugDTO>>> Handle(GetDrugListQuery request, CancellationToken cancellationToken)
        {
            var result = await _drugDataService.ImportDrugDataAsync();
            return _responseHandler.Success(result.AsEnumerable());
        }
    }
    public class CheckInteractionQueryHandler : IRequestHandler<CheckInteractionQuery, Response<InteractionResponseDTO>>
    {
        private readonly IDrugDataService _drugDataService;
        private readonly ResponseHandler _responseHandler;

        public CheckInteractionQueryHandler(IDrugDataService drugDataService, ResponseHandler responseHandler)
        {
            _drugDataService = drugDataService;
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public async Task<Response<InteractionResponseDTO>> Handle(CheckInteractionQuery request, CancellationToken cancellationToken)
        {
            var result = await _drugDataService.CheckDrugInteractionAsync( request.interactionRequestDTO);
            return _responseHandler.Success(result);
        }
    }
    public class GetDrugByIdQueryHandler : IRequestHandler<GetDrugByIdQuery, Response<DrugDTO>>
    {
        private readonly IDrugDataService _drugDataService;
        private readonly ResponseHandler _responseHandler;

        public GetDrugByIdQueryHandler(IDrugDataService drugDataService, ResponseHandler responseHandler)
        {
            _drugDataService = drugDataService;
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public async Task<Response<DrugDTO>> Handle(GetDrugByIdQuery request, CancellationToken cancellationToken)
        {
            var isValid = await _drugDataService.IsValidDrug(request.Id);
            if (!isValid)
                return _responseHandler.NotFound<DrugDTO>("Drug not found.");

            var name = await _drugDataService.GetDrugName(request.Id);
            return _responseHandler.Success(new DrugDTO { Id = request.Id, Name = name });
        }
    }
}
