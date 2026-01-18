using FluentValidation;
using Helix.Core.Features.File.Commands.Models;

namespace Helix.Core.Features.File.Commands.Validation
{
    public class DeleteFileCommandValidator : AbstractValidator<DeleteFileCommand>
    {
        public DeleteFileCommandValidator()
        {
            RuleFor(x => x.FilePath)
                .NotEmpty()
                .WithMessage("File path is required");
        }
    }
}

