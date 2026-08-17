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

namespace Oscar.Infrastructure.Features.ScriptWriter.Queries
{
    public class GetScriptWriterQuery : BaseTableQuery, IRequest<Result<IEntityTable<ScriptWriterDto>>>
    {
        public int Id { get; set; }
    }

    public class GetScriptWriterQueryHandler : AbstractBaseHandler<GetScriptWriterQuery, IEntityTable<ScriptWriterDto>>
    {
        public GetScriptWriterQueryHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetScriptWriterQuery> validator, ILogger<GetScriptWriterQuery> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<IEntityTable<ScriptWriterDto>>> HandleRequest(GetScriptWriterQuery request, CancellationToken cancellationToken)
        {
            Logger.LogInformation((int)ScriptWriterFeatureEvent.Get, CommandResult.SUCCESS);

            var ScriptWriters = OscarContext.ScriptWriters;
            var total = ScriptWriters.Count();

            return Result.Ok(EntityTable<ScriptWriterDto>.Create(ScriptWriters.Select(c => Mapper.Map<ScriptWriterDto>(c))).WithTotal(total));
        }
        
    }
}
