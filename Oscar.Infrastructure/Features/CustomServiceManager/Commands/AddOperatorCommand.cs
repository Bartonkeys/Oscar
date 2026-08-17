using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;

namespace Oscar.Infrastructure.Features.CustomServiceManager.Commands
{
    public class AddOperatorCommand: IRequest<Result<OperatorDto>>
    {
        public OperatorDto OperatorDto { get; set; }
    }

    public class AddOperatorCommandHandler : AbstractBaseHandler<AddOperatorCommand, OperatorDto>
    {
        public AddOperatorCommandHandler(OscarContext oscarContext, IMapper mapper, IValidator<AddOperatorCommand> validator, ILogger<AddOperatorCommand> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<OperatorDto>> HandleRequest(AddOperatorCommand request, CancellationToken cancellationToken)
        {
            var @operator = Mapper.Map<Core.Entities.Operator>(request.OperatorDto);
            await OscarContext.AddAsync(@operator);
            await OscarContext.SaveChangesAsync(cancellationToken);

            Logger.LogInformation((int)ActorFeatureEvent.Add, CommandResult.SUCCESS);
            return Result.Ok(Mapper.Map<OperatorDto>(@operator));
        }
    }
}
