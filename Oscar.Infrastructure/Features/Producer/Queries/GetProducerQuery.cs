using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;

namespace Oscar.Infrastructure.Features.Producer.Queries
{
    public class GetProducerQuery : BaseTableQuery, IRequest<Result<IEntityTable<ProducerDto>>>
    {
        public int Id { get; set; }
    }

    public class GetProducerQueryHandler : AbstractBaseHandler<GetProducerQuery, IEntityTable<ProducerDto>>
    {
        public GetProducerQueryHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetProducerQuery> validator, ILogger<GetProducerQuery> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<IEntityTable<ProducerDto>>> HandleRequest(GetProducerQuery request, CancellationToken cancellationToken)
        {
            Logger.LogInformation((int)ProducerFeatureEvent.Get, CommandResult.SUCCESS);

            var producers = OscarContext.Producers;
            var total = producers.Count();

            return Result.Ok(EntityTable<ProducerDto>.Create(producers.Select(c => Mapper.Map<ProducerDto>(c))).WithTotal(total));
        }
        
    }
}
