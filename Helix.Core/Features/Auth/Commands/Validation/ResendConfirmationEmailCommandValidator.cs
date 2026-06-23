using FluentValidation;
using Helix.Core.Features.Auth.Commands.Models;

namespace Helix.Core.Features.Auth.Commands.Validation
{
    public class ResendConfirmationEmailCommandValidator : AbstractValidator<ResendConfirmationEmailCommand>
    {
        public ResendConfirmationEmailCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotNull()
                .WithMessage("Email is required");

            When(x => x.Email != null, () =>
            {
                RuleFor(x => x.Email)
                    .NotEmpty()
                    .WithMessage("User Email is required");
            });
        }
    }
} 
