using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Oscar.Core.Entities;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;

namespace Oscar.Infrastructure.Features.StandAlone.Commands
{
    public class DeleteStandAloneCommand : IRequest<Result<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteStandAloneCommandHandler : AbstractBaseHandler<DeleteStandAloneCommand, string>
    {
        public DeleteStandAloneCommandHandler(OscarContext oscarContext, IMapper mapper, IValidator<DeleteStandAloneCommand> validator, ILogger<DeleteStandAloneCommand> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<string>> HandleRequest(DeleteStandAloneCommand request, CancellationToken cancellationToken)
        {
            var standAlone = OscarContext.StandAlones
                .Include(e => e.Companies)
                .Include(e => e.Titles)
                .Include(e => e.Conflicts)
                .Include(e => e.WorksType)
                .Include(e => e.AlternativeTitles)
                .Include(e => e.WorksStatusHistory).FirstOrDefault(s => s.Id == request.Id);

            if (standAlone == null)
            {
                Logger.LogInformation((int)StandAloneFeatureEvent.DeleteNotFound, $"Not found {request.Id}");
                return Result.Fail<string>(CommandResult.NOTFOUND);
            }
            WorksHelper.RemoveStandAlone(standAlone, OscarContext);
            await OscarContext.SaveChangesAsync(cancellationToken);

            Logger.LogInformation((int)StandAloneFeatureEvent.Delete, CommandResult.SUCCESS);
            return Result.Ok(CommandResult.SUCCESS);
        }

    }
}
