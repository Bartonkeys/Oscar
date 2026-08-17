using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;

namespace Oscar.Infrastructure.Features.Season.Queries
{
    public class GetSeasonByIdQuery: BaseTableQuery, IRequest<Result<SeasonDto>>
    {
        public int Id { get; set; }
    }

    public class SeasonByIdHandler : AbstractBaseHandler<GetSeasonByIdQuery, SeasonDto>
    {
        private readonly IConfiguration _config;

        public SeasonByIdHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetSeasonByIdQuery> validator, ILogger<GetSeasonByIdQuery> logger, IConfiguration config) : base(oscarContext, mapper, validator, logger)
        {
            _config = config;
        }

        protected override async Task<Result<SeasonDto>> HandleRequest(GetSeasonByIdQuery request, CancellationToken cancellationToken)
        {

            var season = await OscarContext.Seasons
                .AsNoTracking()
                .Include(i => i.Genre)
                .Include(i => i.Documents)
                .Include(i => i.Clients)
                .Include(i => i.Catalogues)
                .Include(i => i.Titles)
                .Include(i => i.Conflicts)
                .Include(i => i.WorksType)
                .Include(i => i.Countries)
                .Include(i => i.Companies)
                .Include(i => i.AlternativeTitles)
                .Include(i => i.Producers)
                .Include(i => i.Directors)
                .Include(i => i.Actors)
                .Include(i => i.Distributors)
                .Include(i => i.ScreenWriters)
                .Include(i => i.ScriptWriters)
                .Include(i => i.WorksStatusHistory)
                .Include(i => i.Episodes)!.ThenInclude(t => t.Titles)
                .Include(i => i.Episodes)!.ThenInclude(t => t.Countries)
                .Include(i => i.Series)
                .Include(sr => sr.SocietyReferences)!.ThenInclude(s => s.Society)
                .Include(cr => cr.ClientReferences)!.ThenInclude(c => c.Client)
                .Include(l => l.Languages)
                .Include(i => i.Registrations!.Where(r => r.RegisterStatus == RegisterStatus.Registered))!.ThenInclude(r => r.Society)
                .Include(i => i.Registrations!.Where(r => r.RegisterStatus == RegisterStatus.Registered))!.ThenInclude(r => r.RegistrationBatch)
                .Include(i => i.ReRegistrations)!.ThenInclude(s => s.Society)
                .Include(i => i.Mandates).ThenInclude(i => i.MandateType)
                .AsSplitQuery()
                .SingleOrDefaultAsync(w => w.Id == request.Id, cancellationToken);

            if (season == null)
                return Result.Fail<SeasonDto>("Not found");

            season.Registrations = season?.Registrations?.OrderByDescending(r => r.DateRegistered).GroupBy(r => r.Society?.Id).Select(x => x.First()).ToList();
            Logger.LogInformation((int)SeasonFeatureEvent.Get, CommandResult.SUCCESS);

            var seasonDto = Mapper.Map<SeasonDto>(season);
            foreach (var doc in seasonDto.Documents)
            {
                doc.PublicUrl = _config["oscarstorage:blob"] + ContainerName.DOCUMENTS + Path.DirectorySeparatorChar + doc.DocumentType.ToString() + Path.DirectorySeparatorChar + doc.FileName;
            }

            return Result.Ok(seasonDto);
        }

    }
}
