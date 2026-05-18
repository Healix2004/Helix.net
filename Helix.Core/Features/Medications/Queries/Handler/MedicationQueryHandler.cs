using Helix.Core.Bases;
using Helix.Core.Features.Medications.Queries.Models;
using Helix.Core.Features.Patients.Queries.Models;
using Helix.Service.DTOs.MedicationDTOs;
using Helix.Service.Interfaces;
using MediatR;

namespace Helix.Core.Features.Medications.Queries.Handler
{
    /// <summary>
    /// Handles <see cref="GetMedicationListQuery"/> and returns all medication records.
    /// </summary>
    public class GetMedicationListQueryHandler : IRequestHandler<GetMedicationListQuery, Response<IEnumerable<MedicationDto>>>
    {
        private readonly IMedicationService _medicationService;
        private readonly ResponseHandler _responseHandler;

        public GetMedicationListQueryHandler(IMedicationService medicationService, ResponseHandler responseHandler)
        {
            _medicationService = medicationService;
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public async Task<Response<IEnumerable<MedicationDto>>> Handle(GetMedicationListQuery request, CancellationToken cancellationToken)
        {
            var result = await _medicationService.GetAllMedicationsAsync();
            return _responseHandler.Success(result);
        }
    }
    public class GetMedicationListForPatientQueryHandler(IMedicationService medicationService, ResponseHandler responseHandler) : IRequestHandler<GetMedicationListForPatientQuery, Response<IEnumerable<MedicationDto>>>
    {
        /// <inheritdoc />
        public async Task<Response<IEnumerable<MedicationDto>>> Handle(GetMedicationListForPatientQuery request, CancellationToken cancellationToken)
        {
            var result = await medicationService.GetPatientMedicationsAsync(request.Id);
            return responseHandler.Success(result);
        }
    }

    /// <summary>
    /// Handles <see cref="GetMedicationByIdQuery"/> and returns a single medication record.
    /// Returns <see cref="System.Net.HttpStatusCode.NotFound"/> if no record exists with the given ID.
    /// </summary>
    public class GetMedicationByIdQueryHandler : IRequestHandler<GetMedicationByIdQuery, Response<MedicationDto>>
    {
        private readonly IMedicationService _medicationService;
        private readonly ResponseHandler _responseHandler;

        public GetMedicationByIdQueryHandler(IMedicationService medicationService, ResponseHandler responseHandler)
        {
            _medicationService = medicationService;
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public async Task<Response<MedicationDto>> Handle(GetMedicationByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _medicationService.GetMedicationByIdAsync(Guid.Parse(request.Id.ToString()));
            if (result == null)
                return _responseHandler.NotFound<MedicationDto>("Medication not found.");
            return _responseHandler.Success(result);
        }
    }
}
