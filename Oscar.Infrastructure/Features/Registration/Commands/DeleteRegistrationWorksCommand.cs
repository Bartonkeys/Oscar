using AutoMapper;
using BartonKeys.Functional;
using EFCore.BulkExtensions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;


namespace Oscar.Infrastructure.Features.Registration.Commands
{
    public class DeleteRegistrationWorksCommand: IRequest<Result<string>>
    {
        public Guid BatchId { get; set; }
        public List<int> WorksIds { get; set; }
    }

    public class DeleteRegistrationWorksCommandHandler : AbstractBaseHandler<DeleteRegistrationWorksCommand, string>
    {
        public DeleteRegistrationWorksCommandHandler(OscarContext oscarContext, IMapper mapper, IValidator<DeleteRegistrationWorksCommand> validator, ILogger<DeleteRegistrationWorksCommand> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<string>> HandleRequest(DeleteRegistrationWorksCommand request, CancellationToken cancellationToken)
        {
            var registrations = OscarContext.Registrations
                .Include(e => e.RegistrationBatch)
                .Where(r => r!.RegistrationBatch!.BatchId == request.BatchId && request.WorksIds.Contains(r!.Works!.Id))
                .ToList();

            if (registrations == null)
            {
                Logger.LogInformation((int)RegistrationFeatureEvent.DeleteNotFound, $"Not found {request.BatchId}");
                return Result.Fail<string>(CommandResult.NOTFOUND);
            }

            await OscarContext.BulkDeleteAsync(registrations);
            await OscarContext.SaveChangesAsync(cancellationToken);

            Logger.LogInformation((int)RegistrationFeatureEvent.Delete, CommandResult.SUCCESS);
            return Result.Ok(CommandResult.SUCCESS);
        }

        
    }
}
