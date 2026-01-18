using FluentValidation;
using Helix.Core.Features.Emails.Commands.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helix.Core.Features.Emails.Commands.Validation
{
    public class SendEmailValidator : AbstractValidator<SendEmailCommand>
    {
        public SendEmailValidator() 
        {
            RuleFor(x=>x.Email )
            .NotNull()
            .WithMessage("Email is required");

            When(x => x.Email != null, () =>
            {
                RuleFor(x => x.Email)
                    .NotEmpty()
                    .WithMessage("Email address is required")
                    .EmailAddress()
                    .WithMessage("Invalid email address format");

                RuleFor(x => x.Message)
                    .NotEmpty()
                    .WithMessage("Message is required");
            });
        }
    }
}
