using Helix.Core.Bases;
using Helix.Core.Features.Facilities.Queries.Models;
using Helix.Service.DTOs.FacilitieDTOs;
using Helix.Service.Interfaces;
using MediatR;

namespace Helix.Core.Features.Facilities.Queries.Handler
{
    /// <summary>
    /// Handles <see cref="GetFacilityListQuery"/> and returns all facility records.
    /// </summary>
    public class GetFacilityListQueryHandler : IRequestHandler<GetFacilityListQuery, Response<IEnumerable<FacilitieDto>>>
    {
        private readonly IFacilitieService _facilitieService;
        private readonly ResponseHandler _responseHandler;

        public GetFacilityListQueryHandler(IFacilitieService facilitieService, ResponseHandler responseHandler)
        {
            _facilitieService = facilitieService;
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public async Task<Response<IEnumerable<FacilitieDto>>> Handle(GetFacilityListQuery request, CancellationToken cancellationToken)
        {
            var result = await _facilitieService.GetAllFacilitiesAsync();
            return _responseHandler.Success(result);
        }
    }

    /// <summary>
    /// Handles <see cref="GetFacilityByIdQuery"/> and returns a single facility record.
    /// Returns <see cref="System.Net.HttpStatusCode.NotFound"/> if no record exists with the given ID.
    /// </summary>
    public class GetFacilityByIdQueryHandler : IRequestHandler<GetFacilityByIdQuery, Response<FacilitieDto>>
    {
        private readonly IFacilitieService _facilitieService;
        private readonly ResponseHandler _responseHandler;

        public GetFacilityByIdQueryHandler(IFacilitieService facilitieService, ResponseHandler responseHandler)
        {
            _facilitieService = facilitieService;
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public async Task<Response<FacilitieDto>> Handle(GetFacilityByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _facilitieService.GetFacilitieByIdAsync(Guid.Parse(request.Id.ToString()));
            if (result == null)
                return _responseHandler.NotFound<FacilitieDto>("Facility not found.");
            return _responseHandler.Success(result);
        }
    }
}
