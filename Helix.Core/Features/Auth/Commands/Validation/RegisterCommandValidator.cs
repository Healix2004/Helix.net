using FluentValidation;
using Helix.Core.Features.Auth.Commands.Models;
using Helix.Service.DTOs.AuthDTOs;

namespace Helix.Core.Features.Auth.Commands.Validation
{
    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(x => x.RegisterDto)
                .NotNull()
                .WithMessage("Registration data is required");

            When(x => x.RegisterDto != null, () =>
            {
                RuleFor(x => x.RegisterDto.Username)
                    .NotEmpty()
                    .WithMessage("Username is required")
                    .MinimumLength(3)
                    .WithMessage("Username must be at least 3 characters long");

                RuleFor(x => x.RegisterDto.Email)
                    .NotEmpty()
                    .WithMessage("Email address is required")
                    .EmailAddress()
                    .WithMessage("Invalid email address format");

                RuleFor(x => x.RegisterDto.ConfirmEmail)
                    .NotEmpty()
                    .WithMessage("Email confirmation is required")
                    .Equal(x => x.RegisterDto.Email)
                    .WithMessage("The email and confirmation email do not match");

                RuleFor(x => x.RegisterDto.Password)
                    .NotEmpty()
                    .WithMessage("Password is required")
                    .MinimumLength(6)
                    .WithMessage("Password must be at least 6 characters long");


                RuleFor(x => x.RegisterDto.FirstName)
                    .NotEmpty()
                    .WithMessage("First name is required")
                    .MaximumLength(100)
                    .WithMessage("First name must not exceed 100 characters");

                RuleFor(x => x.RegisterDto.LastName)
                    .NotEmpty()
                    .WithMessage("Last name is required")
                    .MaximumLength(100)
                    .WithMessage("Last name must not exceed 100 characters");

                RuleFor(x => x.RegisterDto.MobileNumber)
                    .NotEmpty()
                    .WithMessage("Mobile number is required");

                RuleFor(x => x.RegisterDto.PhoneNumber)
                    .NotEmpty()
                    .WithMessage("Phone number is required");

                            });
        }
    }
}

