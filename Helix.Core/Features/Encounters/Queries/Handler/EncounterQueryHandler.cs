using Helix.Core.Bases;
using Helix.Core.Features.Encounters.Queries.Models;
using Helix.Service.DTOs.EncounterDTOs;
using Helix.Service.Interfaces;
using MediatR;

namespace Helix.Core.Features.Encounters.Queries.Handler
{
    /// <summary>
    /// Handles <see cref="GetEncounterListQuery"/> and returns all encounter records.
    /// </summary>
    public class GetEncounterListQueryHandler : IRequestHandler<GetEncounterListQuery, Response<IEnumerable<EncounterDto>>>
    {
        private readonly IEncounterService _encounterService;
        private readonly ResponseHandler _responseHandler;

        public GetEncounterListQueryHandler(IEncounterService encounterService, ResponseHandler responseHandler)
        {
            _encounterService = encounterService;
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public async Task<Response<IEnumerable<EncounterDto>>> Handle(GetEncounterListQuery request, CancellationToken cancellationToken)
        {
            var result = await _encounterService.GetAllEncountersAsync();
            return _responseHandler.Success(result);
        }
    }

    /// <summary>
    /// Handles <see cref="GetEncounterByIdQuery"/> and returns a single encounter record.
    /// Returns <see cref="System.Net.HttpStatusCode.NotFound"/> if no record exists with the given ID.
    /// </summary>
    public class GetEncounterByIdQueryHandler : IRequestHandler<GetEncounterByIdQuery, Response<EncounterDto>>
    {
        private readonly IEncounterService _encounterService;
        private readonly ResponseHandler _responseHandler;

        public GetEncounterByIdQueryHandler(IEncounterService encounterService, ResponseHandler responseHandler)
        {
            _encounterService = encounterService;
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public async Task<Response<EncounterDto>> Handle(GetEncounterByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _encounterService.GetEncounterByIdAsync(Guid.Parse(request.Id.ToString()));
            if (result == null)
                return _responseHandler.NotFound<EncounterDto>("Encounter not found.");
            return _responseHandler.Success(result);
        }
    }
}
