using FluentValidation;
using Oscar.Core.DTOs;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;

namespace Oscar.Infrastructure.Features.Season.Validation
{
    public class SeasonAddDtoValidation : AbstractValidator<SeasonAddDto>
    {

        public SeasonAddDtoValidation(OscarContext context)
        {
            var entityChecker = new EntityChecker(context);
            var validationHelper = new ValidationHelper();

            RuleFor(r => r.Titles).Must(ValidationHelper.HaveAtLeastOneTitle).WithMessage("Must have at least one title");
            RuleFor(r => r.WorksStatus).Must(entityChecker.WorkStatusMustExist).When(r => r.WorksStatus != null).WithMessage("Work status does not exist");

            RuleFor(r => r.SeriesId).NotNull().WithMessage("Series is required");
            RuleFor(r => r.SeriesId).GreaterThan(0).WithMessage("Series is required");

            RuleFor(r => r.SeriesId)
                .NotEqual(0).WithMessage("Series must be a valid value")
                .Must(entityChecker.SeriesMustExist).When(r => r.SeriesId > 0).WithMessage("Series does not exist");

            When(x => x.WorksStatus != Core.Enums.WorksStatus.Uncontrolled, () =>
            {
                RuleFor(r => r.GenreId)
                .NotEqual(0).WithMessage("Genre must be a valid value")
                .Must(entityChecker.GenreMustExist).When(r => r.GenreId != null).WithMessage("Genre does not exist");


                RuleFor(r => r.DurationMinutes).NotEqual(0).WithMessage("Duration minutes must not be equal to 0");

                RuleFor(r => r.ProductionYear)
                    .NotEmpty().WithMessage($"Production year is required")
                    .GreaterThan(1900).WithMessage($"Production year must be greater than 1900")
                    .LessThan(DateTime.Now.Year + 10).WithMessage($"Production year must be less than {DateTime.Now.Year + 10}");

                RuleFor(r => r.FirstBroadcastYear)
                    .GreaterThan(1900).WithMessage($"First broadcast year must be greater than 1900")
                    .LessThan(DateTime.Now.Year + 10).WithMessage($"First broadcast year must be less than {DateTime.Now.Year + 10}");

                RuleFor(r => r.CountryIds).NotEmpty().WithMessage($"At least one production country is required");

                RuleFor(r => r.DirectorIds).NotEmpty().WithMessage($"At least one director is required");

                RuleFor(r => r.CompanyIds).NotEmpty().WithMessage($"At least one production company is required");

                RuleFor(r => r.LanguageIds).NotEmpty().WithMessage($"At least one production language is required");
            });

            When(x => x.WorksStatus == Core.Enums.WorksStatus.Uncontrolled, () =>
            {
                RuleFor(r => r.UncontrolledReason).NotEmpty().WithMessage($"Uncontrolled Work items must have a reason specified");
                RuleFor(r => r.Titles).Must(entityChecker.TitleMustNotExist).When(r => r.Titles != null).WithMessage("Another record uses this Title");
            });
        }

    }
}