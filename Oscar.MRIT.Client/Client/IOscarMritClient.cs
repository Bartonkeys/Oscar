using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BartonKeys.Functional;
using Oscar.MRIT.Core.DTOs;
using Oscar.MRIT.Core.Enums;
using Oscar.MRIT.Core.MRITModels;
using Oscar.Mrit.Features.FelixMrit.Commands;
using Oscar.Mrit.Features.MRITIntegration.Commands;

namespace Oscar.MRIT.Client.Client;

public interface IOscarMritClient
{
    public Task<Result<IEnumerable<ProductionModel>>> GetFeed(int take, MatchStatus matchStatus,
        CancellationToken cancellationToken);

    public Task<Result<IEnumerable<ClientCataloguesDto>>> GetCatalogues();

    public Task<Result<ClientWorksDto>> GetWorks(int clientId);

    public Task<Result<IEnumerable<CatalogueWorksDto>>> PostCatalogueWorks(List<CatalogueDto> catalogues);

    public Task<Result<IEnumerable<ClientCataloguesDto>>> GetWorksClientsAndCatalogues(List<int> worksIds);

    public Task<Result> PostMatches(AddFelixMritMatchesCommand felixMritMatches);

    public Task<Result> PostMatchStatus(UpdateMatchStatusCommand updateMatchStatusCommand);

    public Task<Result<List<MatchStatusDto>>> GetUnsuccessfulMatches();

    public Task<Result<IEnumerable<ProductionModel>>> GetFeed(IEnumerable<int> worksIds);

    public Task<Result> UpdateMatchStatus(IEnumerable<MatchStatusDto> statuses,
        CancellationToken cancellationToken);
}