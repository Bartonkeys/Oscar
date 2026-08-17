using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;

namespace Oscar.Infrastructure.Features.Works.Commands
{
    public class AddWorksCommand: IRequest<Result<WorksDto>>
    {
        public WorksDto WorksDto { get; set; }
    }

    public class AddWorksCommandHandler : AbstractBaseHandler<AddWorksCommand, WorksDto>
    {
        public AddWorksCommandHandler(OscarContext oscarContext, IMapper mapper, IValidator<AddWorksCommand> validator, ILogger<AddWorksCommand> logger) 
            : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<WorksDto>> HandleRequest(AddWorksCommand request, CancellationToken cancellationToken)
        {
            var works = Mapper.Map<Oscar.Core.Entities.Works>(request.WorksDto);
            OscarContext.Add(works);
            await OscarContext.SaveChangesAsync(cancellationToken);

            Logger.LogInformation((int)WorksFeatureEvent.Add, CommandResult.SUCCESS);
            return Result.Ok(Mapper.Map<WorksDto>(works));
        }

    }
}
