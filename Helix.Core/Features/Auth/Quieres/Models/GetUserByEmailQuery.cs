using Helix.Core.Bases;
using Helix.Service.DTOs.AuthDTOs;
using MediatR;

namespace Helix.Core.Features.Auth.Quieres.Models
{
    public class GetUserByEmailQuery : IRequest<Response<UserDto>>
    {
        public string Email { get; set; }

        public GetUserByEmailQuery(string email)
        {
            Email = email;
        }
    }
}

