using FluentValidation;
using Helix.Core.Features.Auth.Commands.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Helix.Core.Features.Auth.Commands.Validation
{
    public class RegisterStep1CommandValidator : AbstractValidator<RegisterStep1Command>
    {
        public RegisterStep1CommandValidator()
        {
            RuleFor(x => x.RegisterStep1Dto)
               .NotNull()
               .WithMessage("Registration data is required");

            When(x => x.RegisterStep1Dto != null, () =>
            {
                RuleFor(x => x.RegisterStep1Dto.Username)
                    .NotEmpty()
                    .WithMessage("Username is required")
                    .MinimumLength(3)
                    .WithMessage("Username must be at least 3 characters long");

                RuleFor(x => x.RegisterStep1Dto.Password)
                    .NotEmpty()
                    .WithMessage("Password is required")
                    .MinimumLength(6)
                    .WithMessage("Password must be at least 6 characters long");
                RuleFor(x => x.RegisterStep1Dto.Email)
                    .NotEmpty()
                    .WithMessage("Email address is required")
                    .EmailAddress()
                    .WithMessage("Invalid email address format");
            });
        }
    }
}
