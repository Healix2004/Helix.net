using Helix.Core.Bases;
using Helix.Core.Features.LabTestResults.Queries.Models;
using Helix.Service.DTOs.LabTestResultDTOs;
using Helix.Service.Interfaces;
using MediatR;

namespace Helix.Core.Features.LabTestResults.Queries.Handler
{
    /// <summary>
    /// Handles <see cref="GetLabTestResultListQuery"/> and returns all lab test result records.
    /// </summary>
    public class GetLabTestResultListQueryHandler : IRequestHandler<GetLabTestResultListQuery, Response<IEnumerable<LabTestResultDto>>>
    {
        private readonly ILabTestResultService _labTestResultService;
        private readonly ResponseHandler _responseHandler;

        public GetLabTestResultListQueryHandler(ILabTestResultService labTestResultService, ResponseHandler responseHandler)
        {
            _labTestResultService = labTestResultService;
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public async Task<Response<IEnumerable<LabTestResultDto>>> Handle(GetLabTestResultListQuery request, CancellationToken cancellationToken)
        {
            var result = await _labTestResultService.GetAllLabTestResultsAsync();
            return _responseHandler.Success(result);
        }
    }

    /// <summary>
    /// Handles <see cref="GetLabTestResultByIdQuery"/> and returns a single lab test result.
    /// Returns <see cref="System.Net.HttpStatusCode.NotFound"/> if no record exists with the given ID.
    /// </summary>
    public class GetLabTestResultByIdQueryHandler : IRequestHandler<GetLabTestResultByIdQuery, Response<LabTestResultDto>>
    {
        private readonly ILabTestResultService _labTestResultService;
        private readonly ResponseHandler _responseHandler;

        public GetLabTestResultByIdQueryHandler(ILabTestResultService labTestResultService, ResponseHandler responseHandler)
        {
            _labTestResultService = labTestResultService;
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public async Task<Response<LabTestResultDto>> Handle(GetLabTestResultByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _labTestResultService.GetLabTestResultByIdAsync(Guid.Parse(request.Id.ToString()));
            if (result == null)
                return _responseHandler.NotFound<LabTestResultDto>("Lab test result not found.");
            return _responseHandler.Success(result);
        }
    }
}
