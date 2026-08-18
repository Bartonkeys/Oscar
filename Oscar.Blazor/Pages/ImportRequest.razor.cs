using Microsoft.AspNetCore.Components;
using MudBlazor;
using Oscar.Blazor.Library.Components;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Infrastructure.Features.Clients.Queries;
using Oscar.Infrastructure.Features.Series.Queries;
using Oscar.Infrastructure.Features.WorksImport.Commands;
using Oscar.Infrastructure.Features.WorksImport.Queries;

namespace Oscar.Blazor.Pages
{
    public partial class ImportRequest
    {
        private IEnumerable<WorksDto> _works;
        private WorksDto _selectedItem;
        private Discriminator _discriminator = Discriminator.Episode;
        private bool _loading = true;
        private bool openImportCreate;

        private IEnumerable<WorksImportRequestDto> pagedData;
        private MudTable<WorksImportRequestDto> table;

        private int totalItems;
        private string searchString = null;
        private List<ClientBasicDto> _clients;

        private int requestId;
        private bool openImport;
        private bool isSuccess;

        private async Task<TableData<WorksImportRequestDto>> ServerReload(TableState state, CancellationToken token)
        {
            table.Loading = true;
            var importsTable = (await Mediator.Send(new GetWorksImportRequestsQuery
            {
                Start = state.Page * state.PageSize,
                Take = state.PageSize
            })).Value;
            _clients = (await Mediator.Send(new GetClientBasicQuery())).Value;

            totalItems = importsTable.TotalRecords;
            pagedData = importsTable.Records.ToArray();
            _loading = false;
            return new TableData<WorksImportRequestDto>() { TotalItems = totalItems, Items = pagedData };
        }

        private void OnSearch(string text)
        {
            _loading = true;
            searchString = text;
            table.ReloadServerData();
        }

        private void OpenImportCreate()
        {
            openImportCreate = true;
            openImport = false;
        }

        private void OpenImportsDrawer(int contextId, WorksImportRequestStatus worksImportRequestStatus)
        {
            requestId = contextId;
            isSuccess = worksImportRequestStatus == WorksImportRequestStatus.Success;
            openImport = true;
            openImportCreate = false;
        }

        private async Task Rollback(int contextId)
        {
            var dialog = await DialogService.ShowAsync<ConfirmDialog>("Rollback Import Request");
            var dialogResult = await dialog.Result;

            if (!dialogResult.Canceled)
            {
                var result = await Mediator.Send(new RollbackWorksImportRequestCommand { Id = contextId });
                if (result.IsSuccess)
                    Snackbar.Add("Rollback Request Submitted", Severity.Success);
                else
                    Snackbar.Add(result.Error, Severity.Error, config => { config.VisibleStateDuration = 9000; });

                await table.ReloadServerData();
            }
        }

        private async Task Refresh()
        {
            await table.ReloadServerData();
        }

        private Color GetColour(WorksImportRequestStatus contextStatus)
        {
            switch (contextStatus)
            {
                case WorksImportRequestStatus.Success:
                    return Color.Success;
                case WorksImportRequestStatus.PossibleDuplicates:
                    return Color.Error;
                case WorksImportRequestStatus.RolledBack:
                    return Color.Warning;
                default:
                    return Color.Info;
            }
        }
    }
}
