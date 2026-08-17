using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;


namespace Oscar.Infrastructure.Features.SocietyReferences.Commands
{
    public class DeleteSocietyReferenceCommand: IRequest<Result<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteSocietyReferenceCommandHandler : AbstractBaseHandler<DeleteSocietyReferenceCommand, string>
    {
        public DeleteSocietyReferenceCommandHandler(OscarContext oscarContext, IMapper mapper, IValidator<DeleteSocietyReferenceCommand> validator, ILogger<DeleteSocietyReferenceCommand> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<string>> HandleRequest(DeleteSocietyReferenceCommand request, CancellationToken cancellationToken)
        {
            var SocietyReference = OscarContext.SocietyReferences
                .FirstOrDefault(s => s.Id == request.Id);

            if (SocietyReference == null)
            {
                Logger.LogInformation((int)SocietyReferenceFeatureEvent.DeleteNotFound, $"Not found {request.Id}");
                return Result.Fail<string>(CommandResult.NOTFOUND);
            }

            OscarContext.SocietyReferences.Remove(SocietyReference);
            await OscarContext.SaveChangesAsync(cancellationToken);

            Logger.LogInformation((int)SocietyReferenceFeatureEvent.Delete, CommandResult.SUCCESS);
            return Result.Ok(CommandResult.SUCCESS);
        }

        
    }
}
