using MediatR;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Oscar.Blazor.Library.Common;
using Oscar.Blazor.Library.Components.Works;
using Oscar.Blazor.Library.Services;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Infrastructure.Features.Clients.Queries;
using Oscar.Infrastructure.Features.Country.Queries;
using Oscar.Infrastructure.Features.Matching.Queries;
using Oscar.Infrastructure.Features.Rights.Queries;

namespace Oscar.Blazor.Pages
{
    public partial class Match: OscarComponentBase
    {
        private bool openMatchCreate;

        private IEnumerable<MatchRequestDto> pagedData;
        private MudTable<MatchRequestDto> table;

        private int totalItems;

        private List<ClientBasicDto> _clients = new();

        private int requestId;
        private List<CountryDto> _countries = new();
        private List<EnumDTO> _matchRules = new();
        private List<RightsTypeDto> _rightsTypes;
        private string _searchString;

        private async Task<TableData<MatchRequestDto>> ServerReload(TableState state, CancellationToken token)
        {
            try
            {
                await SetStatusAsync(true, "Loading Match");
                var matchTable = (await Mediator.Send(new GetMatchRequestsQuery
                {
                    Start = state.Page * state.PageSize,
                    Take = state.PageSize
                })).Value;

                _clients = (await Mediator.Send(new GetClientBasicQuery())).Value;
                _countries = await RefDataService.GetCountries();
                _matchRules = (await Mediator.Send(new GetMatchingStaticDataQuery(Enums.MatchRules))).Value.ToList();
                _rightsTypes = await RefDataService.GetRightsType();

                totalItems = matchTable.TotalRecords;
                pagedData = matchTable.Records.ToArray();
                return new TableData<MatchRequestDto>() { TotalItems = totalItems, Items = pagedData };
            }
            finally
            {
                await SetStatusAsync(false, "Match Loaded");
            }
        }

        private bool Filter(MatchRequestDto item) => FilterBySearchString(item, _searchString);

        private static bool FilterBySearchString(MatchRequestDto item, string searchString)
        {
            return string.IsNullOrWhiteSpace(searchString)
                   || string.IsNullOrWhiteSpace(item.Reference)
                   || item.MatchingResultPublicUrl!.Contains(searchString, StringComparison.OrdinalIgnoreCase);
        }

        private void OpenMatchCreate()
        {
            openMatchCreate = true;
        }

        private async Task Refresh()
        {
            await table.ReloadServerData();
        }

        private Color GetColour(MatchRequestStatus contextStatus)
        {
            switch (contextStatus)
            {
                case MatchRequestStatus.Success:
                    return Color.Success;
                case MatchRequestStatus.Error:
                    return Color.Error;
                case MatchRequestStatus.Pending:
                    return Color.Secondary;
                case MatchRequestStatus.Processing:
                    return Color.Warning;
                default:
                    return Color.Info;
            }
        }
    }
}
