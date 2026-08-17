using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BartonKeys.Functional;
using MediatR;
using Oscar.Mrit.Features.FelixMrit.Commands;
using Oscar.Mrit.Features.MRITIntegration.Commands;
using Oscar.Mrit.Features.MRITIntegration.Queries;
using Oscar.MRIT.Core.DTOs;
using Oscar.MRIT.Core.Enums;
using Oscar.MRIT.Core.MRITModels;

namespace Oscar.MRIT.Client.Client
{
    public class OscarMritClient : IOscarMritClient
    {
        private readonly IMediator _mediator;

        public OscarMritClient(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<Result<IEnumerable<ProductionModel>>> GetFeed(int take, MatchStatus matchStatus,
            CancellationToken cancellationToken) =>
            await _mediator.Send(new FelixWorksQuery { Take = take, MatchStatus = matchStatus }, cancellationToken);

        public async Task<Result<IEnumerable<ClientCataloguesDto>>> GetCatalogues() =>
            await _mediator.Send(new CataloguesByClientQuery());

        public async Task<Result<ClientWorksDto>> GetWorks(int clientId) =>
            await _mediator.Send(new WorksByClientQuery { ClientId = clientId });

        public async Task<Result<IEnumerable<CatalogueWorksDto>>> PostCatalogueWorks(List<CatalogueDto> catalogues) =>
            await _mediator.Send(new WorksByCataloguesQuery() { Catalogues = catalogues });

        public async Task<Result<IEnumerable<ClientCataloguesDto>>> GetWorksClientsAndCatalogues(List<int> worksIds) =>
            await _mediator.Send(new ClientAndCatalogueByWorksQuery() { WorksIds = worksIds });

        public async Task<Result> PostMatches(AddFelixMritMatchesCommand felixMritMatches) =>
            await _mediator.Send(felixMritMatches);

        public async Task<Result> PostMatchStatus(UpdateMatchStatusCommand updateMatchStatusCommand) =>
            await _mediator.Send(updateMatchStatusCommand);

        public async Task<Result<List<MatchStatusDto>>> GetUnsuccessfulMatches() =>
            await _mediator.Send(new UnsuccessfulMatchStatusQuery());

        public async Task<Result<IEnumerable<ProductionModel>>> GetFeed(IEnumerable<int> worksIds) =>
            await _mediator.Send(new WorksByIdQuery { WorksIds = worksIds });

        public async Task<Result> UpdateMatchStatus(IEnumerable<MatchStatusDto> statuses,
            CancellationToken cancellationToken) 
            => await _mediator.Send(new UpdateMatchStatusCommand { Statuses = statuses }, cancellationToken);

    }
}
