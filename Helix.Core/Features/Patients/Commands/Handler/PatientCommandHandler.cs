using Helix.Core.Bases;
using Helix.Core.Features.Patients.Commands.Models;
using Helix.Service.DTOs.AuthDTOs;
using Helix.Service.DTOs.PatientDTOs;
using Helix.Service.Interfaces;
using Helix.Service.Services.AuthServices;
using MediatR;

namespace Helix.Core.Features.Patients.Commands.Handler
{
    /// <summary>
    /// Handles the <see cref="CreatePatientCommand"/> and creates a new patient record.
    /// Returns <see cref="System.Net.HttpStatusCode.Created"/> on success.
    /// </summary>
    public class CreatePatientCommandHandler : IRequestHandler<CreatePatientCommand, Response<PatientDto>>
    {
        private readonly IPatientService _patientService;
        private readonly ResponseHandler _responseHandler;

        public CreatePatientCommandHandler(IPatientService patientService, ResponseHandler responseHandler)
        {
            _patientService = patientService;
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public async Task<Response<PatientDto>> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _patientService.CreatePatientAsync(request.Dto);

                if (result == null)
                {
                    return _responseHandler.BadRequest<PatientDto>("Registration failed. Please check your information and try again.");
                }

                return _responseHandler.Created(result);
            }
            catch (Exception ex)
            {
                return _responseHandler.BadRequest<PatientDto>($"An error occurred during registration: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Handles the <see cref="UpdatePatientCommand"/> and updates an existing patient record.
    /// Returns <see cref="System.Net.HttpStatusCode.NotFound"/> if the patient does not exist.
    /// </summary>
    public class UpdatePatientCommandHandler : IRequestHandler<UpdatePatientCommand, Response<PatientDto>>
    {
        private readonly IPatientService _patientService;
        private readonly ResponseHandler _responseHandler;

        public UpdatePatientCommandHandler(IPatientService patientService, ResponseHandler responseHandler)
        {
            _patientService = patientService;
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public async Task<Response<PatientDto>> Handle(UpdatePatientCommand request, CancellationToken cancellationToken)
        {
            var result = await _patientService.UpdatePatientAsync(Guid.Parse(request.Id.ToString()), request.Dto);
            if (result == null)
                return _responseHandler.NotFound<PatientDto>("Patient not found.");
            return _responseHandler.Success(result);
        }
    }

    /// <summary>
    /// Handles the <see cref="DeletePatientCommand"/> and removes a patient record.
    /// Returns <see cref="System.Net.HttpStatusCode.NotFound"/> if the patient does not exist.
    /// </summary>
    public class DeletePatientCommandHandler : IRequestHandler<DeletePatientCommand, Response<bool>>
    {
        private readonly IPatientService _patientService;
        private readonly ResponseHandler _responseHandler;

        public DeletePatientCommandHandler(IPatientService patientService, ResponseHandler responseHandler)
        {
            _patientService = patientService;
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public async Task<Response<bool>> Handle(DeletePatientCommand request, CancellationToken cancellationToken)
        {
            var result = await _patientService.DeletePatientAsync(Guid.Parse(request.Id.ToString()));
            if (!result)
                return _responseHandler.NotFound<bool>("Patient not found.");
            return _responseHandler.Deleted<bool>();
        }
    }
}
