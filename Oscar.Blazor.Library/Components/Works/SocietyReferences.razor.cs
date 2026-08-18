using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Oscar.Core.DTOs;
using Oscar.Infrastructure.Features.SocietyReferences.Commands;
using Oscar.Infrastructure.Features.SocietyReferences.Queries;

namespace Oscar.Blazor.Library.Components.Works
{
    public partial class SocietyReferences
    {
        private bool _openSocietyReferencesDrawer;

        private SocietyReferenceDto _societyReference { get; set; }

        [Parameter]
        public Expression<Func<ICollection<SocietyReferenceDto>>> For { get; set; }

        [Parameter]
        public ICollection<SocietyReferenceDto> Value { get; set; }

        [Parameter]
        public int WorksId { get; set; }

        [Parameter]
        public int? ClientId { get; set; }

        [Parameter]
        public String Class { get; set; } = "";

        [Parameter]
        public EventCallback<ICollection<SocietyReferenceDto>> SocietyReferencesChanged { get; set; }

        public SocietyReferences()
        {

        }

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            return base.OnAfterRenderAsync(firstRender);
        }

        private async Task DeleteSocietyRefereneAsync(SocietyReferenceDto SocietyReference)
        {
            var dialog = await DialogService.ShowAsync<ConfirmDialog>("Delete SocietyReference?");
            var dialogResult = await dialog.Result;

            if (!dialogResult.Canceled)
            {
                var result = await Mediator.Send(new DeleteSocietyReferenceCommand { Id = SocietyReference.Id });
                if (result.IsSuccess)
                {
                    Value.Remove(SocietyReference);
                    var queryResult = await Mediator.Send(new GetSocietyReferencesByWorksIdQuery { WorksId = WorksId });
                    if (queryResult.IsSuccess)
                    {
                        Value = queryResult.Value;
                        await SocietyReferencesChanged.InvokeAsync(Value);
                        StateHasChanged();
                        Snackbar.Add("SocietyReference deleted", Severity.Success);
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

        protected async Task RefreshSocietyReferences()
        {
            var queryResult = await Mediator.Send(new GetSocietyReferencesByWorksIdQuery { WorksId = WorksId });
            if (queryResult.IsSuccess)
            {
                Value = queryResult.Value;
                await SocietyReferencesChanged.InvokeAsync(Value);
                StateHasChanged();
            }
        }

        protected async Task OpenDrawer(SocietyReferenceDto? SocietyReference)
        {
            if (SocietyReference != null)
                _societyReference = SocietyReference;
            else
                _societyReference = new SocietyReferenceDto();

            _openSocietyReferencesDrawer = true;
            await Task.CompletedTask;
        }

        protected async Task ToggleDrawer(bool open)
        {
            _openSocietyReferencesDrawer = open;
            await Task.CompletedTask;
        }
    }
}
