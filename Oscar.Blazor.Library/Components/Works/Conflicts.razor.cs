using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Oscar.Core.DTOs;
using Oscar.Infrastructure.Features.Conflict.Commands;
using Oscar.Infrastructure.Features.Conflict.Queries;

namespace Oscar.Blazor.Library.Components.Works
{
    public partial class Conflicts
    {
        private bool _openConflictsDrawer;
        private ConflictDto _conflict { get; set; }

        [Parameter]
        public Expression<Func<ICollection<ConflictDto>>> For { get; set; }

        [Parameter]
        public ICollection<ConflictDto> Value { get; set; }

        [Parameter]
        public int WorksId { get; set; }

        [Parameter]
        public int? ClientId { get; set; }

        [Parameter]
        public String Class { get; set; } = "";

        [Parameter]
        public EventCallback<ICollection<ConflictDto>> ConflictsChanged { get; set; }

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            return base.OnAfterRenderAsync(firstRender);
        }

        private async Task DeleteConflictAsync(ConflictDto conflict)
        {
            var dialog = DialogService.Show<ConfirmDialog>("Delete Conflict?");
            var dialogResult = await dialog.Result;

            if (!dialogResult.Cancelled)
            {
                var result = await Mediator.Send(new DeleteConflictCommand { Id = conflict.Id });
                if (result.IsSuccess)
                {
                    Value.Remove(conflict);
                    var queryResult = await Mediator.Send(new GetConflictsByWorksIdQuery { WorksId = WorksId });
                    if (queryResult.IsSuccess)
                    {
                        Value = queryResult.Value;
                        await ConflictsChanged.InvokeAsync(Value);
                        StateHasChanged();
                        Snackbar.Add("Conflict deleted", Severity.Success);
                    }
                    else
                    {
                        Snackbar.Add(queryResult.Error, Severity.Error, config => { config.VisibleStateDuration = 9000; });
                    }
                }
                else
                {
                    Snackbar.Add(result.Error, Severity.Error, config => { config.VisibleStateDuration = 9000; });
                }
            }
        }

        protected async Task RefreshConflicts()
        {
            var queryResult = await Mediator.Send(new GetConflictsByWorksIdQuery { WorksId = WorksId });
            if (queryResult.IsSuccess)
            {
                Value = queryResult.Value;
                await ConflictsChanged.InvokeAsync(Value);
                StateHasChanged();
            }
        }

        protected async Task OpenDrawer(ConflictDto? conflict)
        {
            if (conflict != null)
                _conflict = conflict;
            else
                _conflict = new ConflictDto();

            _openConflictsDrawer = true;
            await Task.CompletedTask;
        }

        protected async Task ToggleDrawer(bool open)
        {
            _openConflictsDrawer = open;
            await Task.CompletedTask;
        }
    }
}
