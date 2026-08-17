using FluentValidation;
using Microsoft.AspNetCore.Http;
using Oscar.Infrastructure.Features.Registration.Commands;

namespace Oscar.Infrastructure.Features.Registration.Validation
{
    public class RegistrationCommandValidator : AbstractValidator<RegistrationCommand>
    {
        public RegistrationCommandValidator()
        {
            RuleFor(b => b.BatchId).NotEqual(Guid.Empty);
            RuleFor(b => b.ClientId).NotEqual(0);
        }
    }

    public class AgicoaRegistrationCommandValidator : AbstractValidator<AgicoaRegistrationCommand>
    {
        public AgicoaRegistrationCommandValidator()
        {
            RuleFor(b => b.BatchId).NotEqual(Guid.Empty);
        }
    }


    public class SuissImageRegistrationCommandValidator : AbstractValidator<SuisseImageCommand>
    {
        public SuissImageRegistrationCommandValidator()
        {
            RuleFor(b => b.BatchId).NotEqual(Guid.Empty);
        }
    }

    public class ScreenrightsRegistrationCommandValidator : AbstractValidator<ScreenrightsRegistrationCommand>
    {
        public ScreenrightsRegistrationCommandValidator()
        {
            RuleFor(b => b.BatchId).NotEqual(Guid.Empty);
        }
    }

    public class ZipRegistrationsCommandValidator : AbstractValidator<StitchOrZipRegistrationsCommand>
    {
        public ZipRegistrationsCommandValidator()
        {
            RuleFor(b => b.BatchId).NotEqual(Guid.Empty);
            RuleFor(b => b.FileResults).NotNull();
        }
    }

    public class CCCRegistrationCommandValidator : AbstractValidator<CCCRegistrationCommand>
    {
        public CCCRegistrationCommandValidator()
        {
            RuleFor(b => b.BatchId).NotEqual(Guid.Empty);
        }
    }

    public class CMCRegistrationCommandValidator : AbstractValidator<CMCRegistrationCommand>
    {
        public CMCRegistrationCommandValidator()
        {
            RuleFor(b => b.BatchId).NotEqual(Guid.Empty);
        }
    }

    public class MPLCRegistrationCommandValidator : AbstractValidator<MPLCRegistrationCommand>
    {
        public MPLCRegistrationCommandValidator()
        {
            RuleFor(b => b.BatchId).NotEqual(Guid.Empty);
        }
    }

    public class CRCRegistrationCommandValidator : AbstractValidator<CRCRegistrationCommand>
    {
        public CRCRegistrationCommandValidator()
        {
            RuleFor(b => b.BatchId).NotEqual(Guid.Empty);
        }
    }

    public class EGEDARegistrationCommandValidator : AbstractValidator<EGEDARegistrationCommand>
    {
        public EGEDARegistrationCommandValidator()
        {
            RuleFor(b => b.BatchId).NotEqual(Guid.Empty);
        }
    }
    public class GWFFRegistrationCommandValidator : AbstractValidator<GWFFRegistrationCommand>
    {
        public GWFFRegistrationCommandValidator()
        {
            RuleFor(b => b.BatchId).NotEqual(Guid.Empty);
        }
    }

    public class MPARegistrationCommandValidator : AbstractValidator<MPARegistrationCommand>
    {
        public MPARegistrationCommandValidator()
        {
            RuleFor(b => b.BatchId).NotEqual(Guid.Empty);
        }
    }

    public class UpfarArgoaRegistrationCommandValidator : AbstractValidator<UpfarArgoaRegistrationCommand>
    {
        public UpfarArgoaRegistrationCommandValidator()
        {
            RuleFor(b => b.BatchId).NotEqual(Guid.Empty);
        }
    }
}
