using FluentValidation;
using Helix.Core.Features.Auth.Commands.Models;

namespace Helix.Core.Features.Auth.Commands.Validation
{
    public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordCommandValidator()
        {
            RuleFor(x => x.ResetPasswordDto)
                .NotNull()
                .WithMessage("Reset password data is required");

            When(x => x.ResetPasswordDto != null, () =>
            {
                RuleFor(x => x.ResetPasswordDto.Email)
                    .NotEmpty()
                    .WithMessage("Email address is required")
                    .EmailAddress()
                    .WithMessage("Invalid email address format");

                RuleFor(x => x.ResetPasswordDto.Token)
                    .NotEmpty()
                    .WithMessage("Token is required");

                RuleFor(x => x.ResetPasswordDto.NewPassword)
                    .NotEmpty()
                    .WithMessage("New password is required")
                    .MinimumLength(8)
                    .WithMessage("Password must be at least 8 characters long");

                RuleFor(x => x.ResetPasswordDto.ConfirmPassword)
                    .NotEmpty()
                    .WithMessage("Password confirmation is required")
                    .Equal(x => x.ResetPasswordDto.NewPassword)
                    .WithMessage("Password and confirmation password do not match");
            });
        }
    }
}

