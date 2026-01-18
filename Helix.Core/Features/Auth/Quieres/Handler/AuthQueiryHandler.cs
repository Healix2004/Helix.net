using AutoMapper;
using Helix.Core.Bases;
using Helix.Core.Features.Auth.Quieres.Models;
using Helix.Data.Entities;
using Helix.Service.DTOs.AuthDTOs;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Helix.Core.Features.Auth.Quieres.Handler
{
    public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, Response<UserDto>>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ResponseHandler _responseHandler;
        private readonly IMapper _mapper;

        public GetUserProfileQueryHandler(UserManager<AppUser> userManager, ResponseHandler responseHandler, IMapper mapper)
        {
            _userManager = userManager;
            _responseHandler = responseHandler;
            _mapper = mapper;
        }

        public async Task<Response<UserDto>> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.UserId))
                {
                    return _responseHandler.BadRequest<UserDto>("User ID is required");
                }

                var user = await _userManager.FindByIdAsync(request.UserId);
                
                if (user == null)
                {
                    return _responseHandler.NotFound<UserDto>("User not found");
                }

                // Map AppUser to UserDto using AutoMapper
                var userDto = _mapper.Map<UserDto>(user);
                return _responseHandler.Success(userDto);
            }
            catch (Exception ex)
            {
                return _responseHandler.BadRequest<UserDto>($"An error occurred while retrieving user profile: {ex.Message}");
            }
        }
    }

    public class GetUserByEmailQueryHandler : IRequestHandler<GetUserByEmailQuery, Response<UserDto>>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ResponseHandler _responseHandler;
        private readonly IMapper _mapper;

        public GetUserByEmailQueryHandler(UserManager<AppUser> userManager, ResponseHandler responseHandler, IMapper mapper)
        {
            _userManager = userManager;
            _responseHandler = responseHandler;
            _mapper = mapper;
        }

        public async Task<Response<UserDto>> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Email))
                {
                    return _responseHandler.BadRequest<UserDto>("Email address is required");
                }

                var user = await _userManager.FindByEmailAsync(request.Email);
                
                if (user == null)
                {
                    return _responseHandler.NotFound<UserDto>("User not found");
                }

                // Map AppUser to UserDto using AutoMapper
                var userDto = _mapper.Map<UserDto>(user);
                return _responseHandler.Success(userDto);
            }
            catch (Exception ex)
            {
                return _responseHandler.BadRequest<UserDto>($"An error occurred while retrieving user: {ex.Message}");
            }
        }
    }
}
