using Helix.Core.Bases;
using Helix.Core.Features.Consents.Commands.Models;
using Helix.Core.Features.Consents.Queries.Models;
using Helix.Service.DTOs.ConsentDTOs;
using Helix.Service.Interfaces;
using MediatR;

namespace Helix.Core.Features.Consents.Queries.Handler
{
    /// <summary>
    /// Handles <see cref="GetConsentListQuery"/> and returns all consent records.
    /// </summary>
    public class GetConsentListQueryHandler : IRequestHandler<GetConsentListQuery, Response<IEnumerable<ConsentDto>>>
    {
        private readonly IConsentService _consentService;
        private readonly ResponseHandler _responseHandler;

        public GetConsentListQueryHandler(IConsentService consentService, ResponseHandler responseHandler)
        {
            _consentService = consentService;
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public async Task<Response<IEnumerable<ConsentDto>>> Handle(GetConsentListQuery request, CancellationToken cancellationToken)
        {
            var result = await _consentService.GetAllConsentsAsync();
            return _responseHandler.Success(result);
        }
    }

    /// <summary>
    /// Handles <see cref="GetConsentByIdQuery"/> and returns a single consent record.
    /// Returns <see cref="System.Net.HttpStatusCode.NotFound"/> if no record exists with the given ID.
    /// </summary>
    public class GetConsentByIdQueryHandler : IRequestHandler<GetConsentByIdQuery, Response<ConsentDto>>
    {
        private readonly IConsentService _consentService;
        private readonly ResponseHandler _responseHandler;

        public GetConsentByIdQueryHandler(IConsentService consentService, ResponseHandler responseHandler)
        {
            _consentService = consentService;
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public async Task<Response<ConsentDto>> Handle(GetConsentByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _consentService.GetConsentByIdAsync(Guid.Parse(request.Id.ToString()));
            if (result == null)
                return _responseHandler.NotFound<ConsentDto>("Consent not found.");
            return _responseHandler.Success(result);
        }
    }
}
