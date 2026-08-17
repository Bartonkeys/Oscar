using System.Linq.Expressions;
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
    public record GetRegistrationWorksQuery: IRequest<Result<HashSet<RegistrationWorksDto>>>
    {
        public int? ClientId { get; set; }
        public int? SocietyId { get; set; }
        public int? CatalogueId { get; set; }

        public bool PreviouslyRegisteredFlag { get; set; }
        public DateTime? FromPreviousRegistration { get; set; }
        public DateTime? ToPreviousRegistration { get; set; }
        public bool IncludeEpisodes { get; set; }
    }

    public class GetRegistrationWorksQueryHandler : AbstractBaseHandler<GetRegistrationWorksQuery, HashSet<RegistrationWorksDto>>
    {
        public GetRegistrationWorksQueryHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetRegistrationWorksQuery> validator, ILogger<GetRegistrationWorksQuery> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<HashSet<RegistrationWorksDto>>> HandleRequest(GetRegistrationWorksQuery request, CancellationToken cancellationToken)
        {
            var standAlones = OscarContext
                .StandAlones
                .Include(t => t.Titles)
                .Where(GetStandAlonePredicate<Core.Entities.StandAlone>(request))
                .Where(r => r.WorksStatus == WorksStatus.Active || r.WorksStatus == WorksStatus.InConflict)
                .Select(sa => new RegistrationWorksDto
                {
                    Id = sa.Id,
                    CompactRef = sa.CompactRef,
                    Title = sa.Titles!.First(t => t.TitleType == TitleType.Main).Title,
                    Discriminator = Discriminator.StandAlone
                }).ToList();

            var series = OscarContext
                .Series
                .Include(t => t.Titles)
                .Include(s => s.Seasons)!.ThenInclude(e => e.Episodes).ThenInclude(r => r.Registrations)
                .Include(s => s.Seasons)!.ThenInclude(e => e.Registrations)
                .Where(GetSeriesPredicate<Core.Entities.Series>(request))
                .Where(r => r.WorksStatus == WorksStatus.Active || r.WorksStatus == WorksStatus.InConflict)
                .Select(series => new RegistrationWorksDto
                {
                    Id = series.Id,
                    Parent = null,
                    CompactRef = series.CompactRef,
                    Title = series.Titles!.First(t => t.TitleType == TitleType.Main).Title,
                    Discriminator = Discriminator.Series,
                    Children = series.Seasons
                        .Where(w => (request.PreviouslyRegisteredFlag && w.Registrations.Any(r => r.DateRegistered >= request.FromPreviousRegistration && r.DateRegistered <= request.ToPreviousRegistration))
                                     || (!request.PreviouslyRegisteredFlag && w.Registrations!.Any(r => r.Society.Id == request.SocietyId && r.RegisterStatus == RegisterStatus.Registered) == false)
                                     || (w.Episodes.Any(e => !e.Registrations.Any(r => r.Society.Id == request.SocietyId && r.RegisterStatus == RegisterStatus.Registered)))
                                     )
                        .Where(r => r.WorksStatus == WorksStatus.Active || r.WorksStatus == WorksStatus.InConflict)
                        .Select(season => new RegistrationWorksDto
                        {
                            Id = season.Id,
                            Parent = new RegistrationWorksDto { Id = series.Id },
                            CompactRef = season.CompactRef,
                            Title = season.Titles!.First(t => t.TitleType == TitleType.Main).Title,
                            Discriminator = Discriminator.Season,
                            Children = request.IncludeEpisodes
                                ? season.Episodes
                                    .Where(w => (request.PreviouslyRegisteredFlag && w.Registrations.Any(r => r.DateRegistered >= request.FromPreviousRegistration && r.DateRegistered <= request.ToPreviousRegistration))
                                                 || (!request.PreviouslyRegisteredFlag && w.Registrations!.Any(r => r.Society.Id == request.SocietyId && r.RegisterStatus == RegisterStatus.Registered) == false)
                                                 )
                                    .Where(r => r.WorksStatus == WorksStatus.Active || r.WorksStatus == WorksStatus.InConflict)
                                    .Select(episode => new RegistrationWorksDto
                                    {
                                        Id = episode.Id,
                                        Parent = new RegistrationWorksDto { Id = season.Id },
                                        CompactRef = episode.CompactRef,
                                        Title = episode.Titles!.First(t => t.TitleType == TitleType.Episode).Title,
                                        Discriminator = Discriminator.Episode,
                                    }).ToHashSet()
                                : new HashSet<RegistrationWorksDto>()
                        }).ToHashSet()
                }).ToHashSet();

            var results = standAlones.Concat(series).OrderBy(x => x.Title).ToHashSet();

            return Result.Ok(results);
        }

        private Expression<Func<T, bool>> GetStandAlonePredicate<T>(GetRegistrationWorksQuery request) where T : Core.Entities.StandAlone
            => request.CatalogueId == null
                ? s => s.Clients.Any(c => c.Id == request.ClientId) && ((request.PreviouslyRegisteredFlag && s.Registrations.Any(r => r.DateRegistered >= request.FromPreviousRegistration && r.DateRegistered <= request.ToPreviousRegistration))
                                                                        || (!request.PreviouslyRegisteredFlag && s.Registrations!.Any(r => r.Society.Id == request.SocietyId
                                                                            && r.Works.Id == s.Id && r.RegisterStatus == RegisterStatus.Registered) == false)
                )
                : s => s.Catalogues.Any(c => c.Id == request.CatalogueId) && ((request.PreviouslyRegisteredFlag && s.Registrations.Any(r => r.DateRegistered >= request.FromPreviousRegistration && r.DateRegistered <= request.ToPreviousRegistration))
                                                                              || (!request.PreviouslyRegisteredFlag && s.Registrations!.Any(r => r.Society.Id == request.SocietyId
                                                                                  && r.Works.Id == s.Id && r.RegisterStatus == RegisterStatus.Registered) == false)
                );


        private Expression<Func<T, bool>> GetSeriesPredicate<T>(GetRegistrationWorksQuery request) where T : Core.Entities.Series
            => request.CatalogueId == null
                ? s => s.Clients.Any(c => c.Id == request.ClientId) && ((request.PreviouslyRegisteredFlag && s.Registrations.Any(r => r.DateRegistered >= request.FromPreviousRegistration && r.DateRegistered <= request.ToPreviousRegistration))
                                                                        || (!request.PreviouslyRegisteredFlag && s.Registrations!.Any(r => r.Society.Id == request.SocietyId
                                                                            && r.Works.Id == s.Id && r.RegisterStatus == RegisterStatus.Registered) == false)
                                                                        || s.Seasons.Any(season => season.Episodes.Any(episode => !episode.Registrations.Any(r => r.Society.Id == request.SocietyId && r.RegisterStatus == RegisterStatus.Registered)))
                                                                        )
                : s => s.Catalogues.Any(c => c.Id == request.CatalogueId) && ((request.PreviouslyRegisteredFlag && s.Registrations.Any(r => r.DateRegistered >= request.FromPreviousRegistration && r.DateRegistered <= request.ToPreviousRegistration))
                                                                              || (!request.PreviouslyRegisteredFlag && s.Registrations!.Any(r => r.Society.Id == request.SocietyId
                                                                                  && r.Works.Id == s.Id && r.RegisterStatus == RegisterStatus.Registered) == false)
                                                                              || s.Seasons.Any(season => season.Episodes.Any(episode => !episode.Registrations.Any(r => r.Society.Id == request.SocietyId && r.RegisterStatus == RegisterStatus.Registered)))
                                                                              );
    }
}
