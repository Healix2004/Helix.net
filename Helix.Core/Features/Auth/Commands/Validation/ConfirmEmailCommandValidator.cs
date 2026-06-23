using FluentValidation;
using Helix.Core.Features.Auth.Commands.Models;

namespace Helix.Core.Features.Auth.Commands.Validation
{
    public class ConfirmEmailCommandValidator : AbstractValidator<ConfirmEmailCommand>
    {
        public ConfirmEmailCommandValidator()
        {
            RuleFor(x => x.ConfirmEmailDto)
                .NotNull()
                .WithMessage("Confirm email data is required");

            When(x => x.ConfirmEmailDto != null, () =>
            {
                RuleFor(x => x.ConfirmEmailDto.UserEmail)
                    .NotEmpty()
                    .WithMessage("User Email is required");

                RuleFor(x => x.ConfirmEmailDto.code)
                    .NotEmpty()
                    .WithMessage("Code  is required");
            });
        }
    }
} 
