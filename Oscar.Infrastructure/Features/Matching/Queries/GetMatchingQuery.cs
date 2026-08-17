using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;
using Oscar.Infrastructure.Features.Common.Contracts;


namespace Oscar.Infrastructure.Features.Matching.Queries
{
    public class GetMatchingQuery: BaseTableQuery, IRequest<Result<IEntityTable<MatchTemplateDto>>>
    {
        public GetMatchingQuery()
        {
        }

    }

    public class MatchingHandler : AbstractBaseHandler<GetMatchingQuery, IEntityTable<MatchTemplateDto>>
    {
        IImporter _importService;
        IExporter _exportService;

        public MatchingHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetMatchingQuery> validator, IImporter importService, IExporter exportService, ILogger<GetMatchingQuery> logger) : base(oscarContext, mapper, validator, logger)
        {
            _importService = importService;
            _exportService = exportService;

        }

        [Obsolete("This method only exists for testing purposes and will be removed - please do not use!")]
        protected override async Task<Result<IEntityTable<MatchTemplateDto>>> HandleRequest(GetMatchingQuery request, CancellationToken cancellationToken)
        {
            var importResult = _importService.ImportMatchCsvAsList(request.SearchObjects.FirstOrDefault().SearchText);
            
            if (importResult.IsFailure)
            {
                Logger.LogError(importResult.ToString());
                return Result.Fail<IEntityTable<MatchTemplateDto>>(importResult.Error);
            }
            
            var matchDtos = importResult.Value;
            var matchDtosForWriting = new List<MatchTemplateResultsDto>();
            foreach (var m in matchDtos)
            {
                var matchDtoForWriting = new MatchTemplateResultsDto();

                matchDtoForWriting.Line = m.Line;
                matchDtoForWriting.Title1 = m.Title1;
                matchDtoForWriting.Title2 = m.Title2;
                matchDtoForWriting.Title3 = m.Title3;
                matchDtoForWriting.SeasonNo = m.SeasonNo;
                matchDtoForWriting.EpisodeNo = m.EpisodeNo;
                matchDtoForWriting.Duration = m.Duration;
                matchDtoForWriting.ShareAvailable = m.ShareAvailable;
                matchDtoForWriting.Director1 = m.Director1;
                matchDtoForWriting.Director2 = m.Director2;
                matchDtoForWriting.ProductionType = m.ProductionType;
                matchDtoForWriting.ProductionCountry = String.Join(";", m.ProductionCountry);
                matchDtoForWriting.Channel = m.Channel;
                matchDtoForWriting.BroadcastDate = m.BroadcastDate;
                matchDtoForWriting.ClientReference = m.ClientReference;
                matchDtoForWriting.OscarClient = "test";
                matchDtoForWriting.OscarDirector = "test";
                matchDtoForWriting.OscarProductionYear = "test";
                matchDtoForWriting.ClientEndDate = "test";
                matchDtoForWriting.MatchingIssue = "test";

                matchDtosForWriting.Add(matchDtoForWriting);

            }

            var exportResult = _exportService.ExportListAsCsv(matchDtosForWriting, Guid.NewGuid().ToString()+".csv");
            if (exportResult.IsFailure)
            {
                Logger.LogError(exportResult.ToString());
                return Result.Fail<IEntityTable<MatchTemplateDto>>(exportResult.Error);
            }

            Logger.LogInformation((int)MatchRequestFeatureEvent.Get, CommandResult.SUCCESS);

            return Result.Ok(EntityTable<MatchTemplateDto>.Create(matchDtos.Select(c => Mapper.Map<MatchTemplateDto>(c))));

        }

    }
}
