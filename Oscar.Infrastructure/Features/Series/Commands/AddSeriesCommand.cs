using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;

namespace Oscar.Infrastructure.Features.Series.Commands
{
    public class AddSeriesCommand: IRequest<Result<SeriesDto>>
    {
        public SeriesAddDto SeriesAddDto { get; set; }
    }

    public class AddSeriesCommandHandler : AbstractBaseHandler<AddSeriesCommand, SeriesDto>
    {
        public AddSeriesCommandHandler(OscarContext oscarContext, IMapper mapper, IValidator<AddSeriesCommand> validator, ILogger<AddSeriesCommand> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<SeriesDto>> HandleRequest(AddSeriesCommand request, CancellationToken cancellationToken)
        {
            foreach (var item in request.SeriesAddDto.Titles.Where(t => t.Id < 0))
                item.Id = 0;

            var series = Mapper.Map<Core.Entities.Series>(request.SeriesAddDto);
            series.Countries = new List<Core.Entities.Country>();
            series.Directors = new List<Core.Entities.Director>();
            series.Languages = new List<Core.Entities.Language>();
            series.Companies = new List<Core.Entities.Company>();
            series.Clients = new List<Core.Entities.Client>();
            series.Catalogues = new List<Core.Entities.Catalogue>();
            series.Mandates = new List<Core.Entities.Mandate>();

            WorksHelper.SetCollection<Core.Entities.Country>(series.Countries, request.SeriesAddDto.CountryIds, OscarContext);
            WorksHelper.SetCollection<Core.Entities.Director>(series.Directors, request.SeriesAddDto.DirectorIds, OscarContext);
            WorksHelper.SetCollection<Core.Entities.Language>(series.Languages, request.SeriesAddDto.LanguageIds, OscarContext);
            WorksHelper.SetCollection<Core.Entities.Company>(series.Companies, request.SeriesAddDto.CompanyIds, OscarContext);
            WorksHelper.SetCollection<Core.Entities.Client>(series.Clients, request.SeriesAddDto.ClientIds, OscarContext);
            WorksHelper.SetCollection<Core.Entities.Catalogue>(series.Catalogues, request.SeriesAddDto.CatalogueIds, OscarContext);
            WorksHelper.SetMandates(series.Mandates, request.SeriesAddDto.MandateTypes, OscarContext);

            series.CompactRef = AutoGenerateCompactRef();

            series.ClientReferences = new List<ClientReference>() { new ClientReference() };

            OscarContext.Add(series);
            await OscarContext.SaveChangesAsync(cancellationToken);

            Logger.LogInformation((int)SeriesFeatureEvent.Add, CommandResult.SUCCESS);
            return Result.Ok(Mapper.Map<SeriesDto>(series));
        }
    }
}
