using FluentValidation;
using Helix.Core.Features.File.Commands.Models;

namespace Helix.Core.Features.File.Commands.Validation
{
    public class UploadMultipleFilesCommandValidator : AbstractValidator<UploadMultipleFilesCommand>
    {
        public UploadMultipleFilesCommandValidator()
        {
            RuleFor(x => x.MultipleFileUploadDto)
                .NotNull()
                .WithMessage("File upload data is required");

            When(x => x.MultipleFileUploadDto != null, () =>
            {
                RuleFor(x => x.MultipleFileUploadDto.files)
                    .NotNull()
                    .WithMessage("Files are required")
                    .Must(files => files != null && files.Count > 0)
                    .WithMessage("At least one file is required")
                    .Must(files => files != null && files.Count <= 10)
                    .WithMessage("Maximum 10 files allowed per upload");

                RuleForEach(x => x.MultipleFileUploadDto.files)
                    .Must(file => file != null && file.Length > 0)
                    .WithMessage("File cannot be empty")
                    .Must(file => file != null && file.Length <= 10 * 1024 * 1024) // 10MB per file
                    .WithMessage("Each file size must not exceed 10MB");
            });
        }
    }
}

