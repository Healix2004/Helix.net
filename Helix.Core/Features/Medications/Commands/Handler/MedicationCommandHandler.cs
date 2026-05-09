using Helix.Core.Bases;
using Helix.Core.Features.Medications.Commands.Models;
using Helix.Service.DTOs.MedicationDTOs;
using Helix.Service.Interfaces;
using MediatR;

namespace Helix.Core.Features.Medications.Commands.Handler
{
    /// <summary>
    /// Handles <see cref="CreateMedicationCommand"/> and creates a new medication record.
    /// </summary>
    public class CreateMedicationCommandHandler : IRequestHandler<CreateMedicationCommand, Response<MedicationDto>>
    {
        private readonly IMedicationService _medicationService;
        private readonly ResponseHandler _responseHandler;

        public CreateMedicationCommandHandler(IMedicationService medicationService, ResponseHandler responseHandler)
        {
            _medicationService = medicationService;
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public async Task<Response<MedicationDto>> Handle(CreateMedicationCommand request, CancellationToken cancellationToken)
        {
            var result = await _medicationService.CreateMedicationAsync(request.Dto);
            return _responseHandler.Created(result);
        }
    }

    /// <summary>
    /// Handles <see cref="UpdateMedicationCommand"/> and updates an existing medication record.
    /// Returns <see cref="System.Net.HttpStatusCode.NotFound"/> if the record does not exist.
    /// </summary>
    public class UpdateMedicationCommandHandler : IRequestHandler<UpdateMedicationCommand, Response<MedicationDto>>
    {
        private readonly IMedicationService _medicationService;
        private readonly ResponseHandler _responseHandler;

        public UpdateMedicationCommandHandler(IMedicationService medicationService, ResponseHandler responseHandler)
        {
            _medicationService = medicationService;
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public async Task<Response<MedicationDto>> Handle(UpdateMedicationCommand request, CancellationToken cancellationToken)
        {
            var result = await _medicationService.UpdateMedicationAsync(Guid.Parse(request.Id.ToString()), request.Dto);
            if (result == null)
                return _responseHandler.NotFound<MedicationDto>("Medication not found.");
            return _responseHandler.Success(result);
        }
    }

    /// <summary>
    /// Handles <see cref="DeleteMedicationCommand"/> and removes a medication record.
    /// Returns <see cref="System.Net.HttpStatusCode.NotFound"/> if the record does not exist.
    /// </summary>
    public class DeleteMedicationCommandHandler : IRequestHandler<DeleteMedicationCommand, Response<bool>>
    {
        private readonly IMedicationService _medicationService;
        private readonly ResponseHandler _responseHandler;

        public DeleteMedicationCommandHandler(IMedicationService medicationService, ResponseHandler responseHandler)
        {
            _medicationService = medicationService;
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public async Task<Response<bool>> Handle(DeleteMedicationCommand request, CancellationToken cancellationToken)
        {
            var result = await _medicationService.DeleteMedicationAsync(Guid.Parse(request.Id.ToString()));
            if (!result)
                return _responseHandler.NotFound<bool>("Medication not found.");
            return _responseHandler.Deleted<bool>();
        }
    }
}
