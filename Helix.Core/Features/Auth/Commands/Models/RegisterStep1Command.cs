using Helix.Core.Bases;
using Helix.Service.DTOs.AuthDTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Helix.Core.Features.Auth.Commands.Models
{
    public class RegisterStep1Command : IRequest<Response<AuthDto>>
    {
        public RegisterStep1Dto RegisterStep1Dto {  get; set; }
        public RegisterStep1Command(RegisterStep1Dto registerStep1Dto)
        {
            this.RegisterStep1Dto = registerStep1Dto;
        }
    }
}
