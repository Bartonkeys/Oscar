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

namespace Oscar.Infrastructure.Features.Registration.Queries
{
    public class GetRegistrationsByWorksIdQuery : BaseTableQuery, IRequest<Result<List<RegistrationDisplayDto>>>
    {
        public int WorksId { get; set; }
    }

    public class GetRegistrationsByWorksIdQueryHandler : AbstractBaseHandler<GetRegistrationsByWorksIdQuery, List<RegistrationDisplayDto>>
    {
        public GetRegistrationsByWorksIdQueryHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetRegistrationsByWorksIdQuery> validator, ILogger<GetRegistrationsByWorksIdQuery> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<List<RegistrationDisplayDto>>> HandleRequest(GetRegistrationsByWorksIdQuery request, CancellationToken cancellationToken)
        {
            OscarContext.ChangeTracker.LazyLoadingEnabled = false;
            var registrations = await OscarContext.Registrations
                .AsNoTracking()
                .Include(r => r.Society)
                .Include(r => r.RegistrationBatch)
                .Include(r => r.Works)
                .Where(r => r.RegisterStatus == RegisterStatus.Registered && r.Works.Id == request.WorksId)
                .ToListAsync();

            if (registrations == null)
                return Result.Fail<List<RegistrationDisplayDto>>("Not found");

            Logger.LogInformation((int)EpisodeFeatureEvent.Get, CommandResult.SUCCESS);
            return Result.Ok(Mapper.Map<List<RegistrationDisplayDto>>(registrations));
        }
    }
}
