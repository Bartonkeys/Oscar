using FluentValidation;
using Microsoft.AspNetCore.Http;
using Oscar.Infrastructure.Features.Matching.Commands;

namespace Oscar.Infrastructure.Features.Matching.Validation
{
    public class AddMatchRequestCommandValidator : AbstractValidator<AddMatchRequestCommand>
    {
        public AddMatchRequestCommandValidator()
        {
            RuleFor(r => r.MatchRequestAddDto).NotNull().WithMessage("Match request required");

            RuleFor(r => r.MatchRequestAddDto.Rules).NotNull().WithMessage("Rules required");
            RuleFor(r => r.MatchRequestAddDto.RequestedBy).NotEmpty().WithMessage("Requested by is required");

            RuleFor(r => r.MatchRequestAddDto.FormFile).NotNull().WithMessage("File is required");
            RuleFor(r => r.MatchRequestAddDto.FormFile).Must(FileIsCSV).WithMessage("File must be in CSV format").When(r => r.MatchRequestAddDto.FormFile != null);
        }

        private bool FileIsCSV(AddMatchRequestCommand addMatchRequestCommand, IFormFile? formFile)
        {
            var extension = Path.GetExtension(formFile?.FileName);
            return String.Equals(extension, ".csv", StringComparison.OrdinalIgnoreCase);   
        }
    }

    
}
