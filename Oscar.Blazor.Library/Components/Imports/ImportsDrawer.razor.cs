using Microsoft.AspNetCore.Components;
using MudBlazor;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Infrastructure.Features.Series.Queries;
using Oscar.Infrastructure.Features.WorksImport.Commands;
using Oscar.Infrastructure.Features.WorksImport.Queries;

namespace Oscar.Blazor.Library.Components.Imports
{
    partial class ImportsDrawer
    {
        private IEnumerable<WorksDto> _works;
        private string _searchString = string.Empty;
        private WorksDto _selectedItem;
        private HashSet<WorksDto> _selectedItems = new();
        private Discriminator _discriminator = Discriminator.Episode;
        private bool _loading = true;

        private IEnumerable<WorksImportDto> pagedData;
        private MudTable<WorksImportDto> table;

        private int totalItems;
        private string searchString = null;

        [Parameter]
        public int id { get; set; }

        [Parameter]
        public bool OpenImportDrawer { get; set; }

        [Parameter]
        public bool IsSuccess { get; set; }

        [Parameter]
        public EventCallback OnReSubmit { get; set; }

        protected override void OnParametersSet()
        {
            if (table != null && OpenImportDrawer)
                table.ReloadServerData();
        }

        protected async Task<TableData<WorksImportDto>> ServerReload(TableState state, CancellationToken token)
        {
            if (!OpenImportDrawer)
                return new TableData<WorksImportDto>()
                {
                    Items = new List<WorksImportDto>(),
                    TotalItems = 0
                };

            table.Loading = true;
            var importsTable = (await Mediator.Send(new GetWorksImportsByRequestIdQuery
            {
                Id = id,
                Start = state.Page * state.PageSize,
                Take = state.PageSize
            })).Value;

            totalItems = importsTable.TotalRecords;
            pagedData = importsTable.Records.ToArray();
            _loading = false;
            return new TableData<WorksImportDto>() { TotalItems = totalItems, Items = pagedData };
        }

        protected void OnSearch(string text)
        {
            _loading = true;
            searchString = text;
            table.ReloadServerData();
        }

        protected async Task Resubmit()
        {
            var result = await Mediator.Send(new ResubmitWorksImportRequestCommand()
            {
                Id = id
            });

            if (result.IsSuccess)
            {
                await table.ReloadServerData();
                IsSuccess = true;
                Snackbar.Add("Resubmit Success", Severity.Success);
                await OnReSubmit.InvokeAsync();
                OpenImportDrawer = false;
            }
            else
            {
                Snackbar.Add(result.Error, Severity.Error, config => { config.VisibleStateDuration = 9000; });
            }
        }

        protected async Task DeleteImport(int contextId)
        {
            var result = await Mediator.Send(new DeleteWorksImportCommand()
            {
                Id = contextId
            });

            if (result.IsSuccess)
            {
                await table.ReloadServerData();
                Snackbar.Add($"Import with id {contextId} removed", Severity.Success);
            }
            else
            {
                Snackbar.Add(result.Error, Severity.Error, config => { config.VisibleStateDuration = 9000; });
            }
        }

        protected Color GetColour(bool contextPossibleDuplicate)
        {
            return contextPossibleDuplicate ? Color.Error : Color.Success;
        }
    }
}
