using Helix.Core.Bases;
using Helix.Core.Features.Allergies.Commands.Models;
using Helix.Core.Features.Allergies.Queries.Models;
using Helix.Service.DTOs.AllergyDTOs;
using Helix.Service.Interfaces;
using MediatR;

namespace Helix.Core.Features.Allergies.Queries.Handler
{
    /// <summary>
    /// Handles <see cref="GetAllergyListQuery"/> and returns all allergy records.
    /// </summary>
    public class GetAllergyListQueryHandler : IRequestHandler<GetAllergyListQuery, Response<IEnumerable<AllergyDto>>>
    {
        private readonly IAllergyService _allergyService;
        private readonly ResponseHandler _responseHandler;

        public GetAllergyListQueryHandler(IAllergyService allergyService, ResponseHandler responseHandler)
        {
            _allergyService = allergyService;
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public async Task<Response<IEnumerable<AllergyDto>>> Handle(GetAllergyListQuery request, CancellationToken cancellationToken)
        {
            var result = await _allergyService.GetAllAllergiesAsync();
            return _responseHandler.Success(result);
        }
    }
    public class GetAllergyListForPatientQueryHandler(IAllergyService allergyService, ResponseHandler responseHandler) : IRequestHandler<GetAllergyListForPatientQuery, Response<IEnumerable<AllergyDto>>>
    {
        /// <inheritdoc />
        public async Task<Response<IEnumerable<AllergyDto>>> Handle(GetAllergyListForPatientQuery request, CancellationToken cancellationToken)
        {
            var result = await allergyService.GetPatientAllergiesAsync(request.Id);
            return responseHandler.Success(result);
        }
    }

    /// <summary>
    /// Handles <see cref="GetAllergyByIdQuery"/> and returns a single allergy record.
    /// Returns <see cref="System.Net.HttpStatusCode.NotFound"/> if no record exists with the given ID.
    /// </summary>
    public class GetAllergyByIdQueryHandler : IRequestHandler<GetAllergyByIdQuery, Response<AllergyDto>>
    {
        private readonly IAllergyService _allergyService;
        private readonly ResponseHandler _responseHandler;

        public GetAllergyByIdQueryHandler(IAllergyService allergyService, ResponseHandler responseHandler)
        {
            _allergyService = allergyService;
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public async Task<Response<AllergyDto>> Handle(GetAllergyByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _allergyService.GetAllergyByIdAsync(Guid.Parse(request.Id.ToString()));
            if (result == null)
                return _responseHandler.NotFound<AllergyDto>("Allergy not found.");
            return _responseHandler.Success(result);
        }
    }
}
