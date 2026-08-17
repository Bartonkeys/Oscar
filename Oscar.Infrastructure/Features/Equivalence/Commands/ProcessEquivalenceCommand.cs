using System.Collections.Concurrent;
using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;
using Oscar.Infrastructure.Features.Common.Contracts;
using Polly;
using Polly.Retry;

namespace Oscar.Infrastructure.Features.Equivalence.Commands
{
    public class ProcessEquivalenceCommand : IRequest<Result<string>>
    {
        public Guid RequestId { get; set; }
    }

    public class EquivalenceCommandHandler : AbstractBaseHandler<ProcessEquivalenceCommand,string>
    {
        private IExporter _exporter;
        private IImporter _importer;
        private readonly ConcurrentBag<EquivalenceRequestDto> _exportList = new();
        private readonly ConcurrentBag<string> _registrationErrors = new();
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly AsyncRetryPolicy _retryPolicy;

        public EquivalenceCommandHandler(
            OscarContext oscarContext,
            IMapper mapper,
            IValidator<ProcessEquivalenceCommand> validator,
            IExporter exporter,
            IImporter importer,
            IServiceScopeFactory serviceScopeFactory,
            ILogger<ProcessEquivalenceCommand> logger, 
            IMediator mediator)
            : base(oscarContext, mapper, validator, logger)
        {
            _exporter = exporter;
            _importer = importer;
            _serviceScopeFactory = serviceScopeFactory;
            _mapper = mapper;
            _mediator = mediator;

            _retryPolicy = Policy
                .Handle<DbUpdateException>()
                .Or<SqlException>()
                .WaitAndRetryAsync(
                    3, // Retry 3 times
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), // Exponential backoff
                    (exception, timeSpan, retryCount, context) =>
                    {
                        Logger.LogWarning($"Retry {retryCount} encountered an exception: {exception.Message}. Waiting {timeSpan} before next retry. ");
                    });
        }

        protected override async Task<Result<string>> HandleRequest(ProcessEquivalenceCommand request, CancellationToken cancellationToken)
        {
            var equivalenceRequest = await OscarContext.EquivalenceRequests.FirstOrDefaultAsync(m => m.RequestID == request.RequestId, cancellationToken: cancellationToken);
            if (equivalenceRequest == null)
            {
                Logger.LogInformation((int)EquivalenceRequestFeatureEvent.DocumentNotFound, $"Not found {request.RequestId}");
                return Result.Fail<string>(CommandResult.NOTFOUND);
            }

            equivalenceRequest.EquivalenceRequestStatus = EquivalenceRequestStatus.Processing;
            await OscarContext.SaveChangesAsync(cancellationToken);

            var importResult = _importer.ImportEquivalenceCsvAsList($"{equivalenceRequest.FileName}");
            if (importResult.IsFailure)
            {
                Logger.LogError((int)EquivalenceRequestFeatureEvent.DocumentNotFound, $"Request id -- {request.RequestId} -- Document '{equivalenceRequest.FileName}' not found in storage: {importResult.Error}");
                equivalenceRequest.EquivalenceRequestStatus = EquivalenceRequestStatus.Failed;
                await OscarContext.SaveChangesAsync(cancellationToken);
                return Result.Fail<string>(CommandResult.NOTFOUND);
            }

            try
            {
                foreach (var chunk in importResult.Value.Chunk(Constants.Default.ThreadSize))
                {
                    var tasks = chunk.Select(equivalenceDto => Task.Run(() => ProcessEquivalence(equivalenceDto, request, cancellationToken), cancellationToken));
                    await Task.WhenAll(tasks).ConfigureAwait(false);
                }

                var equivalenceReportResult = _exporter.ExportEquivalenceListAsCsv($"{equivalenceRequest.FileName}_result", importResult.Value);

                if (equivalenceReportResult.IsSuccess)
                    equivalenceRequest.Url = equivalenceReportResult.Value;

                equivalenceRequest.EquivalenceRequestStatus = EquivalenceRequestStatus.Processed;
                await OscarContext.SaveChangesAsync(cancellationToken);

                return Result.Ok(equivalenceRequest.ToString());
            }
            catch (Exception ex)
            {
                Logger.LogError((int)FunctionEvent.EquivalenceError, $"Request id -- {request.RequestId} -- Processing error with document '{equivalenceRequest.FileName}': {ex.ToString()}");
                return Result.Fail<string>(CommandResult.ERROR);
            }
        }

        private async Task ProcessEquivalence(EquivalenceDto equivalence, ProcessEquivalenceCommand request, CancellationToken cancellationToken)
        {
            await _retryPolicy.ExecuteAsync(async () =>
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var scopedContext = scope.ServiceProvider.GetRequiredService<OscarContext>();
                Logger.LogInformation(
                    $"Request id -- {request.RequestId} -- AGICOA_NO: {equivalence.AGICOA_NO} - DECLARATION_ID: {equivalence.DECLARATION_ID} - ISAN_WORK: {equivalence.ISAN_WORK} - Compact Ref: {equivalence.YOUR_REFERENCE}");

                if (string.IsNullOrEmpty(equivalence.YOUR_REFERENCE))
                {
                    equivalence.DECLARATION_STATUS = $"Not Matched  -- No CompactRef set for: {equivalence.AGICOA_NO}";
                    Logger.LogInformation($"Request id -- {request.RequestId} -- {equivalence.DECLARATION_STATUS}");
                    return;
                }

                var compactRef = GetCompactRef(equivalence);

                if (string.IsNullOrEmpty(compactRef))
                {
                    equivalence.STATUS =
                        $"Not Matched  -- Incorrect format for CompactRef: {equivalence.YOUR_REFERENCE}";
                    Logger.LogInformation($"Request id -- {request.RequestId} -- {equivalence.STATUS}");
                    return;
                }

                Logger.LogInformation($"Request id -- {request.RequestId} -- Searching for CompactRef: {compactRef}");

                var matchingWork = await scopedContext.Works
                    .Include(t => t.Titles)
                    .Include(w => w.Clients)
                    .Include(w => w.ClientReferences)
                    .FirstOrDefaultAsync(w => w.CompactRef != null && w.CompactRef.Equals(compactRef),
                        cancellationToken: cancellationToken);

                if (matchingWork == null)
                {
                    equivalence.DECLARATION_STATUS = $"Not Matched  -- No match found for CompactRef: {compactRef}";
                    Logger.LogInformation($"Request id -- {request.RequestId} -- {equivalence.DECLARATION_STATUS}");
                    return;
                }

                equivalence.CLIENT = matchingWork.Clients!.FirstOrDefault()?.ClientName;
                equivalence.TITLE = matchingWork.Titles!.FirstOrDefault(t => t.TitleType is TitleType.Main or TitleType.Episode)?.Title;

                Logger.LogInformation(
                    $"Request id -- {request.RequestId} -- Match found for CompactRef: {compactRef} updating with Isan {equivalence.ISAN_WORK} and Agicoa Works Reference {equivalence.AGICOA_NO}");

                if (!string.IsNullOrEmpty(equivalence.ISAN_WORK))
                {
                    equivalence.ISIN_STATUS = equivalence.ISAN_WORK == matchingWork.Isan
                        ? $"ISAN Code ignored for {equivalence.ISAN_WORK}"
                        : string.IsNullOrEmpty(matchingWork.Isan)
                            ? $"ISAN Code Added - {equivalence.ISAN_WORK}"
                            : $"ISAN Code Updated - {equivalence.ISAN_WORK}";
                    matchingWork.Isan = equivalence.ISAN_WORK;
                }

                if (!string.IsNullOrEmpty(equivalence.AGICOA_NO))
                {
                    equivalence.AGICOA_NO_STATUS += equivalence.AGICOA_NO == matchingWork.AgicoaWorksReference
                        ? $"AGICOA_NO Code ignored for {equivalence.AGICOA_NO}"
                        : string.IsNullOrEmpty(matchingWork.AgicoaWorksReference)
                            ? $"AGICOA_NO Code Added - {equivalence.AGICOA_NO}"
                            : $"AGICOA_NO Code Updated - {equivalence.AGICOA_NO}";
                    matchingWork.AgicoaWorksReference = equivalence.AGICOA_NO;
                }

                if (!string.IsNullOrEmpty(equivalence.DECLARATION_ID))
                {
                    var clientReference = matchingWork.ClientReferences!.FirstOrDefault();

                    if (clientReference != null)
                    {
                        equivalence.DECLARATION_STATUS =
                            equivalence.DECLARATION_ID == clientReference.AgicoaDeclarationNumber
                                ? $"Declaration id ignored for {equivalence.DECLARATION_ID}"
                                : string.IsNullOrEmpty(clientReference.AgicoaDeclarationNumber)
                                    ? $"Declaration No Added - {equivalence.DECLARATION_ID}"
                                    : $"Declaration No updated - {equivalence.DECLARATION_ID}";

                        Logger.LogInformation( $"Request id -- {request.RequestId} -- Update with declaration id {equivalence.DECLARATION_ID}");

                        clientReference.AgicoaDeclarationNumber = equivalence.DECLARATION_ID;
                    }
                    else
                    {
                        equivalence.DECLARATION_STATUS = "No Client Reference found";
                    }
                }

                await scopedContext.SaveChangesAsync(cancellationToken);
            });
        }

        private string GetCompactRef(EquivalenceDto equivalence)
        {
            var splitReference = equivalence.YOUR_REFERENCE!.Split('-');

            switch (splitReference.Length)
            {
                case >= 2:
                    return splitReference[1];
                case 1 when splitReference[0].Length == 8:
                    return splitReference[0];
                default:
                    return string.Empty;
            }
        }
    }
}
