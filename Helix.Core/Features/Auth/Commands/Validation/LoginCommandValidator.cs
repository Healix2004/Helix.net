using FluentValidation;
using Helix.Core.Features.Auth.Commands.Models;

namespace Helix.Core.Features.Auth.Commands.Validation
{
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.LoginDto)
                .NotNull()
                .WithMessage("Login data is required");

            When(x => x.LoginDto != null, () =>
            {
                RuleFor(x => x.LoginDto.EmailAddress)
                    .NotEmpty()
                    .WithMessage("Email address is required")
                    .EmailAddress()
                    .WithMessage("Invalid email address format");

                RuleFor(x => x.LoginDto.Password)
                    .NotEmpty()
                    .WithMessage("Password is required");
            });
        }
    }
}

