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

namespace Oscar.Infrastructure.Features.Series.Queries
{
    public class GetSeriesByIdQuery: BaseTableQuery, IRequest<Result<SeriesDto>>
    {
        public int Id { get; set; }
    }

    public class SeriesByIdHandler : AbstractBaseHandler<GetSeriesByIdQuery, SeriesDto>
    {
        private readonly IConfiguration _config;

        public SeriesByIdHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetSeriesByIdQuery> validator, ILogger<GetSeriesByIdQuery> logger, IConfiguration config) : base(oscarContext, mapper, validator, logger)
        {
            _config = config;
        }

        protected override async Task<Result<SeriesDto>> HandleRequest(GetSeriesByIdQuery request, CancellationToken cancellationToken)
        {

            var series = await OscarContext.Series
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
                .Include(i => i.Seasons)!.ThenInclude(t => t.Titles)
                .Include(i => i.Seasons)!.ThenInclude(t => t.Countries)
                .Include(i => i.Episodes)!.ThenInclude(t => t.Titles)
                .Include(sr => sr.SocietyReferences)!.ThenInclude(s => s.Society)
                .Include(cr => cr.ClientReferences)!.ThenInclude(c => c.Client)
                .Include(i => i.Languages)
                .Include(i => i.Registrations!.Where(r => r.RegisterStatus == RegisterStatus.Registered))!.ThenInclude(r => r.Society)
                .Include(i => i.Registrations!.Where(r => r.RegisterStatus == RegisterStatus.Registered))!.ThenInclude(r => r.RegistrationBatch)
                .Include(i => i.ReRegistrations)!.ThenInclude(s => s.Society)
                .Include(i => i.Mandates).ThenInclude(i => i.MandateType)
                .AsSplitQuery()
                .SingleOrDefaultAsync(w => w.Id == request.Id, cancellationToken);

            if (series == null)
                return Result.Fail<SeriesDto>("Not found");

            series.Registrations = series?.Registrations?.OrderByDescending(r => r.DateRegistered).GroupBy(r => r.Society?.Id).Select(x => x.First()).ToList();

            Logger.LogInformation((int)SeriesFeatureEvent.Get, CommandResult.SUCCESS);

            var seriesDto = Mapper.Map<SeriesDto>(series);
            foreach (var doc in seriesDto.Documents)
            {
                doc.PublicUrl = _config["oscarstorage:blob"] + ContainerName.DOCUMENTS + Path.DirectorySeparatorChar + doc.DocumentType.ToString() + Path.DirectorySeparatorChar + doc.FileName;
            }

            return Result.Ok(seriesDto);
        }
    }
}
