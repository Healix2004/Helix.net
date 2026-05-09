using Helix.Core.Bases;
using Helix.Core.Features.Drugs.Queries.Models;
using Helix.Service.DTOs.DrugDTOs;
using Helix.Service.Interfaces;
using MediatR;

namespace Helix.Core.Features.Drugs.Queries.Handler
{
    /// <summary>
    /// Handles <see cref="GetDrugListQuery"/> by importing the full drug list from the external drug data service.
    /// </summary>
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

    /// <summary>
    /// Handles <see cref="GetDrugByIdQuery"/> by validating the drug ID and retrieving the drug name
    /// from the external drug data service.
    /// Returns <see cref="System.Net.HttpStatusCode.NotFound"/> if the drug ID is not recognised.
    /// </summary>
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
