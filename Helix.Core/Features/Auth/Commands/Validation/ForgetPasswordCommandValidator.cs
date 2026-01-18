using FluentValidation;
using Helix.Core.Features.Auth.Commands.Models;

namespace Helix.Core.Features.Auth.Commands.Validation
{
    public class ForgetPasswordCommandValidator : AbstractValidator<ForgetPasswordCommand>
    {
        public ForgetPasswordCommandValidator()
        {
            RuleFor(x => x.ForgetPasswordDto)
                .NotNull()
                .WithMessage("Forget password data is required");

            When(x => x.ForgetPasswordDto != null, () =>
            {
                RuleFor(x => x.ForgetPasswordDto.Email)
                    .NotEmpty()
                    .WithMessage("Email address is required")
                    .EmailAddress()
                    .WithMessage("Invalid email address format");
            });
        }
    }
}

