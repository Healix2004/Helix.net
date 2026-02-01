using Helix.Core.Bases;
using Helix.Core.Features.Auth.Commands.Models;
using Helix.Core.Features.Auth.Commands.Validation;
using Helix.Service.DTOs.AuthDTOs;
using Helix.Service.Interfaces;
using MediatR;

namespace Helix.Core.Features.Auth.Commands.Handler
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Response<AuthDto>>
    {
        private readonly IAuthService _authService;
        private readonly ResponseHandler _responseHandler;

        public RegisterCommandHandler(IAuthService authService, ResponseHandler responseHandler)
        {
            _authService = authService;
            _responseHandler = responseHandler;
        }

        public async Task<Response<AuthDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _authService.RegisterAsync(request.RegisterDto);
                
                if (result == null || string.IsNullOrEmpty(result.AccessToken))
                {
                    return _responseHandler.BadRequest<AuthDto>("Registration failed. Please check your information and try again.");
                }

                return _responseHandler.Created(result);
            }
            catch (Exception ex)
            {
                return _responseHandler.BadRequest<AuthDto>($"An error occurred during registration: {ex.Message}");
            }
        }
    }
    public class RegisterStep1CommandHandler : IRequestHandler<RegisterStep1Command, Response<AuthDto>>
    {
        private readonly IAuthService _authService;
        private readonly ResponseHandler _responseHandler;

        public RegisterStep1CommandHandler(IAuthService authService, ResponseHandler responseHandler)
        {
            _authService = authService;
            _responseHandler = responseHandler;
        }

        public async Task<Response<AuthDto>> Handle(RegisterStep1Command request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _authService.RegisterStep1Async(request.RegisterStep1Dto);
                
                if (result == null || string.IsNullOrEmpty(result.AccessToken))
                {
                    return _responseHandler.BadRequest<AuthDto>("Registration failed. Please check your information and try again.");
                }

                return _responseHandler.Created(result);
            }
            catch (Exception ex)
            {
                return _responseHandler.BadRequest<AuthDto>($"An error occurred during registration: {ex.Message}");
            }
        }
    }
    public class RegisterDoctorCommandHandler : IRequestHandler<RegisterDoctorCommand, Response<AuthDto>>
    {
        private readonly IAuthService _authService;
        private readonly ResponseHandler _responseHandler;

        public RegisterDoctorCommandHandler(IAuthService authService, ResponseHandler responseHandler)
        {
            _authService = authService;
            _responseHandler = responseHandler;
        }

        public async Task<Response<AuthDto>> Handle(RegisterDoctorCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _authService.RegisterDoctorAsync(request.RegisterDoctorDto);
                
                if (result == null || string.IsNullOrEmpty(result.AccessToken))
                {
                    return _responseHandler.BadRequest<AuthDto>("Registration failed. Please check your information and try again.");
                }

                return _responseHandler.Created(result);
            }
            catch (Exception ex)
            {
                return _responseHandler.BadRequest<AuthDto>($"An error occurred during registration: {ex.Message}");
            }
        }
    }

    public class LoginCommandHandler : IRequestHandler<LoginCommand, Response<AuthDto>>
    {
        private readonly IAuthService _authService;
        private readonly ResponseHandler _responseHandler;

        public LoginCommandHandler(IAuthService authService, ResponseHandler responseHandler)
        {
            _authService = authService;
            _responseHandler = responseHandler;
        }

        public async Task<Response<AuthDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _authService.LoginAsync(request.LoginDto);
                
                if (result == null || string.IsNullOrEmpty(result.AccessToken))
                {
                    return _responseHandler.Unauthorized<AuthDto>();
                }

                return _responseHandler.Success(result);
            }
            catch (InvalidOperationException ex)
            {
                return _responseHandler.BadRequest<AuthDto>(ex.Message);
            }
            catch (Exception ex)
            {
                return _responseHandler.BadRequest<AuthDto>($"An error occurred during login: {ex.Message}");
            }
        }
    }

    public class ForgetPasswordCommandHandler : IRequestHandler<ForgetPasswordCommand, Response<string>>
    {
        private readonly IAuthService _authService;
        private readonly ResponseHandler _responseHandler;

        public ForgetPasswordCommandHandler(IAuthService authService, ResponseHandler responseHandler)
        {
            _authService = authService;
            _responseHandler = responseHandler;
        }

        public async Task<Response<string>> Handle(ForgetPasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _authService.ForgetPasswordAsync(request.ForgetPasswordDto.Email);
                return _responseHandler.Success(result);
            }
            catch (ArgumentException ex)
            {
                return _responseHandler.BadRequest<string>(ex.Message);
            }
            catch (Exception ex)
            {
                return _responseHandler.BadRequest<string>($"An error occurred: {ex.Message}");
            }
        }
    }

    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Response<string>>
    {
        private readonly IAuthService _authService;
        private readonly ResponseHandler _responseHandler;

        public ResetPasswordCommandHandler(IAuthService authService, ResponseHandler responseHandler)
        {
            _authService = authService;
            _responseHandler = responseHandler;
        }

        public async Task<Response<string>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _authService.ResetPasswordAsync(request.ResetPasswordDto);
                return _responseHandler.Success(result);
            }
            catch (ArgumentException ex)
            {
                return _responseHandler.BadRequest<string>(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return _responseHandler.BadRequest<string>(ex.Message);
            }
            catch (Exception ex)
            {
                return _responseHandler.BadRequest<string>($"An error occurred: {ex.Message}");
            }
        }
    }

    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Response<string>>
    {
        private readonly IAuthService _authService;
        private readonly ResponseHandler _responseHandler;

        public ChangePasswordCommandHandler(IAuthService authService, ResponseHandler responseHandler)
        {
            _authService = authService;
            _responseHandler = responseHandler;
        }

        public async Task<Response<string>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _authService.ChangePasswordAsync(
                    request.UserId,
                    request.ChangePasswordDto.CurrentPassword,
                    request.ChangePasswordDto.NewPassword);
                return _responseHandler.Success(result);
            }
            catch (ArgumentException ex)
            {
                return _responseHandler.BadRequest<string>(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return _responseHandler.BadRequest<string>(ex.Message);
            }
            catch (Exception ex)
            {
                return _responseHandler.BadRequest<string>($"An error occurred: {ex.Message}");
            }
        }
    }

    public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, Response<string>>
    {
        private readonly IAuthService _authService;
        private readonly ResponseHandler _responseHandler;

        public ConfirmEmailCommandHandler(IAuthService authService, ResponseHandler responseHandler)
        {
            _authService = authService;
            _responseHandler = responseHandler;
        }

        public async Task<Response<string>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _authService.ConfirmEmailAsync(
                    request.ConfirmEmailDto.UserId,
                    request.ConfirmEmailDto.Token);
                return _responseHandler.Success(result);
            }
            catch (ArgumentException ex)
            {
                return _responseHandler.BadRequest<string>(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return _responseHandler.BadRequest<string>(ex.Message);
            }
            catch (Exception ex)
            {
                return _responseHandler.BadRequest<string>($"An error occurred: {ex.Message}");
            }
        }
    }
}
