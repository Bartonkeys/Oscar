using System.Collections.Concurrent;
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
using Oscar.Infrastructure.Features.Common.Contracts;
using Oscar.Infrastructure.Features.Matching.Contracts;

namespace Oscar.Infrastructure.Features.Matching.Commands
{
    public class MatchCommand: IRequest<Result<string>>
    {
        public string Reference { get; set; }
    }

    public class MatchCommandHandler : AbstractBaseHandler<MatchCommand, string>
    {
        
        private IImporter _importer;
        private IExporter _exporter;
        private IMatchingService _matchingService;
        private readonly ConcurrentBag<MatchTemplateResultsDto> _exportList = new();
        private readonly ConcurrentBag<string> _matchingErrors = new();

        public MatchCommandHandler(
            OscarContext oscarContext, 
            IMapper mapper, 
            IValidator<MatchCommand> validator, 
            IImporter importer, 
            IExporter exporter,
            IMatchingService matchingService,
            ILogger<MatchCommand> logger) 
            : base(oscarContext, mapper, validator, logger)
        {
            _importer = importer;
            _exporter = exporter;
            _matchingService = matchingService;
        }

        protected override async Task<Result<string>> HandleRequest(MatchCommand request, CancellationToken cancellationToken)
        {
            var matchRequest =  await OscarContext.MatchRequests.FirstOrDefaultAsync(m => m.Reference == request.Reference);
            if(matchRequest == null)
            {
                Logger.LogInformation((int)MatchRequestFeatureEvent.UpdateNotFound, $"Not found {request.Reference}");
                return Result.Fail<string>(CommandResult.NOTFOUND);
            }

            if(matchRequest.Status != MatchRequestStatus.Pending)
                return Result.Ok(CommandResult.SUCCESS);

            matchRequest.Status = MatchRequestStatus.Processing;
            await OscarContext.SaveChangesAsync(cancellationToken);

            var importResult = _importer.ImportMatchCsvAsList($"{matchRequest.Reference}.csv");
            if (importResult.IsFailure)
            {
                Logger.LogError((int)MatchRequestFeatureEvent.DocumentNotFound, $"Document '{matchRequest.Reference}' not found in storage: {importResult.Error}");
                return Result.Fail<string>(CommandResult.NOTFOUND);
            }
            _matchingService.LoadRules(
                matchRequest.Rules, 
                matchRequest.ClientId, 
                matchRequest.TerritoryId,
                matchRequest.RightsTypeId, 
                matchRequest.RightsFromYear, 
                matchRequest.RightsToYear, 
                matchRequest.IgnoreCharactersFollowing);

            foreach (var chunk in importResult.Value.Chunk(Constants.Default.ThreadSize))
                await Task.WhenAll(chunk.Select(matchTemplateDto => Task.Run(() => ProcessMatch(matchTemplateDto), cancellationToken)).ToArray()).ConfigureAwait(false);

            if (_matchingErrors.Count > 0)
            {
                matchRequest.Status = MatchRequestStatus.Error;
                await OscarContext.SaveChangesAsync(cancellationToken);
                Logger.LogError((int)MatchRequestFeatureEvent.DocumentNotExported, string.Join(", ", _matchingErrors.Select(x => x)));
                return Result.Fail<string>(string.Join(", ", _matchingErrors.Select(x => x)));
            }
            
            var exportResult = _exporter.ExportListAsCsv(_exportList, $"{matchRequest.Reference}_MATCHED.csv");
            if (exportResult.IsFailure)
            {
                matchRequest.Status = MatchRequestStatus.Error;
                await OscarContext.SaveChangesAsync(cancellationToken);
                Logger.LogError((int)MatchRequestFeatureEvent.DocumentNotExported, exportResult.ToString());
                return Result.Fail<string>(exportResult.Error);
            }

            matchRequest.Status = MatchRequestStatus.Success;
            await OscarContext.SaveChangesAsync(cancellationToken);

            return Result.Ok(CommandResult.SUCCESS);

        }

        private async Task ProcessMatch(MatchTemplateDto matchTemplateDto)
        {
            Logger.LogInformation($"Begin match progress on {matchTemplateDto.Title1} on {System.Environment.CurrentManagedThreadId}");
            var matchResult = await _matchingService.Match(matchTemplateDto);
            if (matchResult.IsFailure)
            {
               _matchingErrors.Add(matchResult.Error);
            }
            _exportList.Add(matchResult.Value);
            Logger.LogInformation($"End match process on {matchTemplateDto.Title1}");
        }
    }
}
