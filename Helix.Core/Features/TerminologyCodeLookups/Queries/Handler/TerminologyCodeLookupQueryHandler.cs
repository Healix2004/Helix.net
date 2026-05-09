using Helix.Core.Bases;
using Helix.Core.Features.TerminologyCodeLookups.Queries.Models;
using Helix.Service.DTOs.TerminologyCodeLookupDTOs;
using Helix.Service.Interfaces;
using MediatR;

namespace Helix.Core.Features.TerminologyCodeLookups.Queries.Handler
{
    /// <summary>
    /// Handles <see cref="GetTerminologyCodeLookupListQuery"/> and returns all terminology code lookup records.
    /// </summary>
    public class GetTerminologyCodeLookupListQueryHandler : IRequestHandler<GetTerminologyCodeLookupListQuery, Response<IEnumerable<TerminologyCodeLookupDto>>>
    {
        private readonly ITerminologyCodeLookupService _terminologyService;
        private readonly ResponseHandler _responseHandler;

        public GetTerminologyCodeLookupListQueryHandler(ITerminologyCodeLookupService terminologyService, ResponseHandler responseHandler)
        {
            _terminologyService = terminologyService;
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public async Task<Response<IEnumerable<TerminologyCodeLookupDto>>> Handle(GetTerminologyCodeLookupListQuery request, CancellationToken cancellationToken)
        {
            var result = await _terminologyService.GetAllTerminologyCodeLookupsAsync();
            return _responseHandler.Success(result);
        }
    }

    /// <summary>
    /// Handles <see cref="GetTerminologyCodeLookupByIdQuery"/> and returns a single terminology code lookup.
    /// Returns <see cref="System.Net.HttpStatusCode.NotFound"/> if no record exists with the given ID.
    /// </summary>
    public class GetTerminologyCodeLookupByIdQueryHandler : IRequestHandler<GetTerminologyCodeLookupByIdQuery, Response<TerminologyCodeLookupDto>>
    {
        private readonly ITerminologyCodeLookupService _terminologyService;
        private readonly ResponseHandler _responseHandler;

        public GetTerminologyCodeLookupByIdQueryHandler(ITerminologyCodeLookupService terminologyService, ResponseHandler responseHandler)
        {
            _terminologyService = terminologyService;
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public async Task<Response<TerminologyCodeLookupDto>> Handle(GetTerminologyCodeLookupByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _terminologyService.GetTerminologyCodeLookupByIdAsync(Guid.Parse(request.Id.ToString()));
            if (result == null)
                return _responseHandler.NotFound<TerminologyCodeLookupDto>("Terminology code lookup not found.");
            return _responseHandler.Success(result);
        }
    }
}
