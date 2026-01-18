using FluentValidation;
using Helix.Core.Features.Auth.Commands.Models;

namespace Helix.Core.Features.Auth.Commands.Validation
{
    public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordCommandValidator()
        {
            RuleFor(x => x.ChangePasswordDto)
                .NotNull()
                .WithMessage("Change password data is required");

            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("User ID is required");

            When(x => x.ChangePasswordDto != null, () =>
            {
                RuleFor(x => x.ChangePasswordDto.CurrentPassword)
                    .NotEmpty()
                    .WithMessage("Current password is required");

                RuleFor(x => x.ChangePasswordDto.NewPassword)
                    .NotEmpty()
                    .WithMessage("New password is required")
                    .MinimumLength(8)
                    .WithMessage("Password must be at least 8 characters long");

                RuleFor(x => x.ChangePasswordDto.ConfirmPassword)
                    .NotEmpty()
                    .WithMessage("Password confirmation is required")
                    .Equal(x => x.ChangePasswordDto.NewPassword)
                    .WithMessage("Password and confirmation password do not match");
            });
        }
    }
}

