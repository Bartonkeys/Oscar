using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;

namespace Oscar.Infrastructure.Features.Episode.Queries
{
    public class GetEpisodeByIdQuery: BaseTableQuery, IRequest<Result<EpisodeDto>>
    {
        public int Id { get; set; }
    }

    public class EpisodeByIdHandler : AbstractBaseHandler<GetEpisodeByIdQuery, EpisodeDto>
    {
        private readonly IConfiguration _config;

        public EpisodeByIdHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetEpisodeByIdQuery> validator, ILogger<GetEpisodeByIdQuery> logger, IConfiguration config) : base(oscarContext, mapper, validator, logger)
        {
            _config = config;
        }

        protected override async Task<Result<EpisodeDto>> HandleRequest(GetEpisodeByIdQuery request, CancellationToken cancellationToken)
        {
            OscarContext.ChangeTracker.LazyLoadingEnabled = false;
            var episode = await OscarContext.Episodes
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
                .Include(i => i.Season)
                .Include(i => i.Series)
                .Include(sr => sr.SocietyReferences)!.ThenInclude(s => s.Society)
                .Include(cr => cr.ClientReferences)!.ThenInclude(c => c.Client)
                .Include(l => l.Languages)
                .Include(i => i.Registrations!.Where(r => r.RegisterStatus == RegisterStatus.Registered))!.ThenInclude(r => r.Society)
                .Include(i => i.Registrations!.Where(r => r.RegisterStatus == RegisterStatus.Registered))!.ThenInclude(r => r.RegistrationBatch)
                .Include(i => i.ReRegistrations)!.ThenInclude(s => s.Society)
                .Include(i => i.Mandates).ThenInclude(i => i.MandateType)
                .AsSplitQuery()
                .FirstOrDefaultAsync(w => w.Id == request.Id, cancellationToken);

            if (episode == null)
                return Result.Fail<EpisodeDto>("Not found");

            episode.Registrations = episode?.Registrations?.OrderByDescending(r => r.DateRegistered).GroupBy(r => r.Society?.Id).Select(x => x.First()).ToList();
            Logger.LogInformation((int)EpisodeFeatureEvent.Get, CommandResult.SUCCESS);

            var episodeDto = Mapper.Map<EpisodeDto>(episode);
            foreach (var doc in episodeDto.Documents)
            {
                doc.PublicUrl = _config["oscarstorage:blob"] + ContainerName.DOCUMENTS + Path.DirectorySeparatorChar + doc.DocumentType.ToString() + Path.DirectorySeparatorChar + doc.FileName;
            }

            return Result.Ok(episodeDto);
        }

    }
}
