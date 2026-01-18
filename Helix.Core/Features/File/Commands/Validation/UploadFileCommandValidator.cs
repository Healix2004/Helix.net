using FluentValidation;
using Helix.Core.Features.File.Commands.Models;

namespace Helix.Core.Features.File.Commands.Validation
{
    public class UploadFileCommandValidator : AbstractValidator<UploadFileCommand>
    {
        public UploadFileCommandValidator()
        {
            RuleFor(x => x.FileUploadDto)
                .NotNull()
                .WithMessage("File upload data is required");

            When(x => x.FileUploadDto != null, () =>
            {
                RuleFor(x => x.FileUploadDto.file)
                    .NotNull()
                    .WithMessage("File is required")
                    .Must(file => file != null && file.Length > 0)
                    .WithMessage("File cannot be empty")
                    .Must(file => file != null && file.Length <= 10 * 1024 * 1024) // 10MB
                    .WithMessage("File size must not exceed 10MB");
            });
        }
    }
}

