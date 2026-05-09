using Helix.Core.Bases;
using Helix.Core.Features.Doctors.Commands.Models;
using Helix.Service.DTOs.DoctorDTOs;
using Helix.Service.Interfaces;
using MediatR;

namespace Helix.Core.Features.Doctors.Commands.Handler
{
    /// <summary>
    /// Handles the <see cref="CreateDoctorCommand"/> and creates a new doctor record.
    /// Returns <see cref="System.Net.HttpStatusCode.Created"/> on success.
    /// </summary>
    public class CreateDoctorCommandHandler : IRequestHandler<CreateDoctorCommand, Response<DoctorDto>>
    {
        private readonly IDoctorService _doctorService;
        private readonly ResponseHandler _responseHandler;

        public CreateDoctorCommandHandler(IDoctorService doctorService, ResponseHandler responseHandler)
        {
            _doctorService = doctorService;
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public async Task<Response<DoctorDto>> Handle(CreateDoctorCommand request, CancellationToken cancellationToken)
        {
            var result = await _doctorService.CreateDoctorAsync(request.Dto);
            return _responseHandler.Created(result);
        }
    }

    /// <summary>
    /// Handles the <see cref="UpdateDoctorCommand"/> and updates an existing doctor record.
    /// Returns <see cref="System.Net.HttpStatusCode.NotFound"/> if the doctor does not exist.
    /// </summary>
    public class UpdateDoctorCommandHandler : IRequestHandler<UpdateDoctorCommand, Response<DoctorDto>>
    {
        private readonly IDoctorService _doctorService;
        private readonly ResponseHandler _responseHandler;

        public UpdateDoctorCommandHandler(IDoctorService doctorService, ResponseHandler responseHandler)
        {
            _doctorService = doctorService;
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public async Task<Response<DoctorDto>> Handle(UpdateDoctorCommand request, CancellationToken cancellationToken)
        {
            var result = await _doctorService.UpdateDoctorAsync(Guid.Parse(request.Id.ToString()), request.Dto);
            if (result == null)
                return _responseHandler.NotFound<DoctorDto>("Doctor not found.");
            return _responseHandler.Success(result);
        }
    }

    /// <summary>
    /// Handles the <see cref="DeleteDoctorCommand"/> and removes a doctor record.
    /// Returns <see cref="System.Net.HttpStatusCode.NotFound"/> if the doctor does not exist.
    /// </summary>
    public class DeleteDoctorCommandHandler : IRequestHandler<DeleteDoctorCommand, Response<bool>>
    {
        private readonly IDoctorService _doctorService;
        private readonly ResponseHandler _responseHandler;

        public DeleteDoctorCommandHandler(IDoctorService doctorService, ResponseHandler responseHandler)
        {
            _doctorService = doctorService;
            _responseHandler = responseHandler;
        }

        /// <inheritdoc />
        public async Task<Response<bool>> Handle(DeleteDoctorCommand request, CancellationToken cancellationToken)
        {
            var result = await _doctorService.DeleteDoctorAsync(Guid.Parse(request.Id.ToString()));
            if (!result)
                return _responseHandler.NotFound<bool>("Doctor not found.");
            return _responseHandler.Deleted<bool>();
        }
    }
}
