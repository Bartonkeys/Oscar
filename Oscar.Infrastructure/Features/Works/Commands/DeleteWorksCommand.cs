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
    public class DeleteWorksCommand : IRequest<Result<WorksDto>>
    {
        public WorksDto WorksDto { get; set; }
    }

    public class DeleteWorksCommandHandler : AbstractBaseHandler<DeleteWorksCommand, WorksDto>
    {
        public DeleteWorksCommandHandler(OscarContext oscarContext, IMapper mapper, 
            IValidator<DeleteWorksCommand> validator, ILogger<DeleteWorksCommand> logger) 
            : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<WorksDto>> HandleRequest(DeleteWorksCommand request, CancellationToken cancellationToken)
        {
            var worksDto = Mapper.Map<Oscar.Core.Entities.Works>(request.WorksDto);
            var worksEntity = OscarContext.Works.FirstOrDefault(x => x.Id == worksDto.Id);

            if (worksEntity == null)
            {
                return Result.Fail<WorksDto>("Works not found");
            }

            OscarContext.Works.Remove(worksEntity);
            await OscarContext.SaveChangesAsync();

            Logger.LogInformation((int)WorksFeatureEvent.Delete, CommandResult.SUCCESS);
            return Result.Ok(Mapper.Map<WorksDto>(worksDto));
        }

    }
}

