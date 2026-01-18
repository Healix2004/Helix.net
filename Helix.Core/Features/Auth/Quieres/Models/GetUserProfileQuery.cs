using Helix.Core.Bases;
using Helix.Service.DTOs.AuthDTOs;
using MediatR;

namespace Helix.Core.Features.Auth.Quieres.Models
{
    public class GetUserProfileQuery : IRequest<Response<UserDto>>
    {
        public string UserId { get; set; }

        public GetUserProfileQuery(string userId)
        {
            UserId = userId;
        }
    }
}

