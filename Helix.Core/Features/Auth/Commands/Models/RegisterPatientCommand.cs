using Helix.Core.Bases;
using Helix.Service.DTOs.AuthDTOs;
using Helix.Service.DTOs.PatientDTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Helix.Core.Features.Auth.Commands.Models
{
    public class RegisterPatientCommand : IRequest<Response<AuthDto>>
    {
        public PatientRegistrationPayloadDto RegisterPatientDto {  get; set; }
        public RegisterPatientCommand(PatientRegistrationPayloadDto registerPatientDto)
        {
            this.RegisterPatientDto = registerPatientDto;
        }
    }
}
