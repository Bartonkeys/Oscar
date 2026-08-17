using AutoMapper;
using BartonKeys.Functional;
using EntityFramework.Exceptions.Common;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;

namespace Oscar.Infrastructure.Features.Works.Commands
{
    public class UpdateWorksCommand : IRequest<Result<WorksDto>>
    {
        public WorksDto WorksDto { get; set; }
    }

    public class UpdateWorksCommandHandler : AbstractBaseHandler<UpdateWorksCommand, WorksDto>
    {
        public UpdateWorksCommandHandler(OscarContext oscarContext, IMapper mapper, 
            IValidator<UpdateWorksCommand> validator, ILogger<UpdateWorksCommand> logger) 
            : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<WorksDto>> HandleRequest(UpdateWorksCommand request, CancellationToken cancellationToken)
        {
            var works = Mapper.Map<Oscar.Core.Entities.Works>(request.WorksDto);

            if (OscarContext.Works.All(x => x.Id != works.Id))
            {
                return Result.Fail<WorksDto>("Works not found");
            }

            OscarContext.Update(works);

            try
            {
                await OscarContext.SaveChangesAsync(cancellationToken);
            }
            catch (UniqueConstraintException e)
            {
                Logger.LogInformation((int)WorksFeatureEvent.Update, CommandResult.ERROR);
                var errorString = e.Message;
                if (e.InnerException != null)
                {
                    errorString += " : " + e.InnerException.Message;
                }
                return Result.Fail<WorksDto>(errorString);
            }

            Logger.LogInformation((int)WorksFeatureEvent.Update, CommandResult.SUCCESS);

            return Result.Ok(Mapper.Map<WorksDto>(works));
        }

    }
}

