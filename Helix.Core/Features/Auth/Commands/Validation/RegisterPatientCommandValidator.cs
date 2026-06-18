using FluentValidation;
using Helix.Core.Features.Auth.Commands.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Helix.Core.Features.Auth.Commands.Validation
{
    public class RegisterPatientCommandValidator : AbstractValidator<RegisterPatientCommand>
    {
        public RegisterPatientCommandValidator()
        {
            RuleFor(x => x.RegisterPatientDto)
               .NotNull()
               .WithMessage("Registration data is required");

            When(x => x.RegisterPatientDto != null, () =>
            {
                RuleFor(x => x.RegisterPatientDto.AccountDetails.Username)
                    .NotEmpty()
                    .WithMessage("Username is required")
                    .MinimumLength(3)
                    .WithMessage("Username must be at least 3 characters long");

                RuleFor(x => x.RegisterPatientDto.AccountDetails.Password)
                    .NotEmpty()
                    .WithMessage("Password is required")
                    .MinimumLength(6)
                    .WithMessage("Password must be at least 6 characters long");
                RuleFor(x => x.RegisterPatientDto.AccountDetails.Email)
                    .NotEmpty()
                    .WithMessage("Email address is required")
                    .EmailAddress()
                    .WithMessage("Invalid email address format");
            });
        }
    }
}
