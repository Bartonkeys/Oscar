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

namespace Oscar.Infrastructure.Features.Society.Queries
{
    public class GetSocietyQuery : IRequest<Result<SocietyDto>>
    {
        public int Id { get; set; }
    }

    public class GetSocietyQueryHandler : AbstractBaseHandler<GetSocietyQuery, SocietyDto>
    {
        public GetSocietyQueryHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetSocietyQuery> validator, ILogger<GetSocietyQuery> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<SocietyDto>> HandleRequest(GetSocietyQuery request, CancellationToken cancellationToken)
        {
            Logger.LogInformation((int)SocietyFeatureEvent.Get, CommandResult.SUCCESS);

            var society = await OscarContext.Societies
                .AsNoTracking()
                .Include(s => s.Clients)
                .Include(c => c.Contacts)
                .Include(c => c.Addresses)
                .Include(c => c.SocietyRights)!.ThenInclude(r => r.RightsType)
                .Include(c => c.SocietyRights)!.ThenInclude(r => r.Country)
                .AsSplitQuery()
                .SingleOrDefaultAsync(s => s.Id == request.Id, cancellationToken: cancellationToken);

            return Result.Ok(Mapper.Map<SocietyDto>(society));
        }
        
    }
}
