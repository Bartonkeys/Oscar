using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;


namespace Oscar.Infrastructure.Features.Conflict.Commands
{
    public class DeleteConflictCommand: IRequest<Result<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteConflictCommandHandler : AbstractBaseHandler<DeleteConflictCommand, string>
    {
        public DeleteConflictCommandHandler(OscarContext oscarContext, IMapper mapper, IValidator<DeleteConflictCommand> validator, ILogger<DeleteConflictCommand> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<string>> HandleRequest(DeleteConflictCommand request, CancellationToken cancellationToken)
        {
            var Conflict = OscarContext.Conflicts
                .FirstOrDefault(s => s.Id == request.Id);

            if (Conflict == null)
            {
                Logger.LogInformation((int)ConflictFeatureEvent.DeleteNotFound, $"Not found {request.Id}");
                return Result.Fail<string>(CommandResult.NOTFOUND);
            }

            OscarContext.Conflicts.Remove(Conflict);
            await OscarContext.SaveChangesAsync(cancellationToken);

            Logger.LogInformation((int)ConflictFeatureEvent.Delete, CommandResult.SUCCESS);
            return Result.Ok(CommandResult.SUCCESS);
        }

        
    }
}
