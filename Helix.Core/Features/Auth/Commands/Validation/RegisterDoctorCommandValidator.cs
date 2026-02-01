using FluentValidation;
using Helix.Core.Features.Auth.Commands.Models;

namespace Helix.Core.Features.Auth.Commands.Validation
{
    public class RegisterDoctorCommandValidator : AbstractValidator<RegisterDoctorCommand>
    {
        public RegisterDoctorCommandValidator()
        {
            RuleFor(x => x.RegisterDoctorDto)
               .NotNull()
               .WithMessage("Registration data is required");

            When(x => x.RegisterDoctorDto != null, () =>
            {
                RuleFor(x => x.RegisterDoctorDto.Username)
                    .NotEmpty()
                    .WithMessage("Username is required")
                    .MinimumLength(3)
                    .WithMessage("Username must be at least 3 characters long");

                RuleFor(x => x.RegisterDoctorDto.Password)
                    .NotEmpty()
                    .WithMessage("Password is required")
                    .MinimumLength(6)
                    .WithMessage("Password must be at least 6 characters long");
                RuleFor(x => x.RegisterDoctorDto.Email)
                    .NotEmpty()
                    .WithMessage("Email address is required")
                    .EmailAddress()
                    .WithMessage("Invalid email address format");
            });
        }
    }
}
