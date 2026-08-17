using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using BartonKeys.Extensions;
using BartonKeys.Functional;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Oscar.MRIT.Core.DTOs;
using Oscar.MRIT.Core.Enums;
using Oscar.MRIT.Core.MRITModels;
using Oscar.Mrit.Features.MRITIntegration.Commands;
using Oscar.Mrit.Features.MRITIntegration.Queries;

namespace Oscar.Mrit.Features.Integrator.Commands
{
    public class FelixOnMusicIntegrationCommand : IRequest<Result>
    {
    }

    public class FelixOnMusicIntegrationCommandHandler : IRequestHandler<FelixOnMusicIntegrationCommand, Result>
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _mritHttpClient;
        private readonly ILogger<FelixOnMusicIntegrationCommandHandler> _logger;
        private readonly IMediator _mediator;

        public FelixOnMusicIntegrationCommandHandler(IHttpClientFactory httpClientFactory, ILogger<FelixOnMusicIntegrationCommandHandler> logger, IMediator mediator)
        {
            _httpClientFactory = httpClientFactory;
            _mritHttpClient = _httpClientFactory.CreateClient("mrit");
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<Result> Handle(FelixOnMusicIntegrationCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start processing productions");
            var successResult = await ProcessFor(MatchStatus.Success, cancellationToken);
            _logger.LogInformation("Retry errors");
            var errorResult = await ProcessFor(MatchStatus.Error, cancellationToken);
            _logger.LogInformation("Retry duplicates");
            var duplicateResult = await ProcessFor(MatchStatus.Duplicate, cancellationToken);

            return Result.Combine(successResult, errorResult, duplicateResult);
        }

        private async Task<Result> ProcessFor(MatchStatus matchStatus, CancellationToken cancellationToken)
        {
            while (true)
            {
                var felixResult = await GetFelixFeed(cancellationToken, matchStatus);
                if (felixResult.IsFailure)
                    return Result.Fail(felixResult.Error);

                if (!felixResult.Value.Any()) break;

                var mritResult = await SendToMrit(felixResult, cancellationToken);
                if (mritResult.IsFailure)
                    return Result.Fail(mritResult.Error);
            }

            return Result.Ok();
        }

        private async Task<Result<IEnumerable<ProductionModel>>> GetFelixFeed(CancellationToken cancellationToken, MatchStatus matchStatus = MatchStatus.Success) 
            => await _mediator.Send(new FelixWorksQuery { Take = 20, MatchStatus = matchStatus }, cancellationToken);

        private async Task<Result> SendToMrit(Result<IEnumerable<ProductionModel>> felixResult, CancellationToken cancellationToken)
        {
            var statuses = BuildStatusesFrom(felixResult).ToList();

            var mritResult = await PostToMrit(felixResult.Value, cancellationToken);
            if (mritResult.IsFailure)
                return Result.Fail(mritResult.Error);

            foreach (var felixExceptionReport in mritResult.Value)
            {
                statuses.FirstOrDefault(s => s.WorksId == felixExceptionReport.WorksIds)
                    .ToMaybe()
                    .IfSome(s =>
                    {
                        s.MatchStatus = felixExceptionReport.Error.Contains("Duplicate Match") ? MatchStatus.Duplicate : MatchStatus.Error;
                        s.Message = felixExceptionReport.Error;
                    });
            }

            var statusResult = await UpdateMatchStatus(statuses, cancellationToken);
            if (statusResult.IsFailure)
                return Result.Fail(statusResult.Error);

            return Result.Ok();
        }

        private IEnumerable<MatchStatusDto?> BuildStatusesFrom(Result<IEnumerable<ProductionModel>> felixResult)
        {
            return felixResult.Value.Select(p => new MatchStatusDto
                {WorksId = p.Id, MatchStatus = MatchStatus.Success}).ToList();
        }

        private async Task<Result<List<FelixExceptionReport>>> PostToMrit(IEnumerable<ProductionModel> felixFeed,
            CancellationToken cancellationToken)
        {
            try
            {
                var mritResponse =
                    await _mritHttpClient.PostAsJsonAsync("/api/Felix/input/", felixFeed, cancellationToken);

                return !mritResponse.IsSuccessStatusCode
                    ? Result.Fail<List<FelixExceptionReport>>(mritResponse.ReasonPhrase)
                    : Result.Ok(
                        JsonConvert.DeserializeObject<List<FelixExceptionReport>>(
                            await mritResponse.Content.ReadAsStringAsync(cancellationToken)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Result.Fail < List < FelixExceptionReport >> (ex.Message);
            }
        }

        private async Task<Result> UpdateMatchStatus(IEnumerable<MatchStatusDto?> statuses,
            CancellationToken cancellationToken) => await _mediator.Send(new UpdateMatchStatusCommand { Statuses = statuses }, cancellationToken);

    }
}