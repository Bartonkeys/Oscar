using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Oscar.Core.DTOs;
using Oscar.Infrastructure.Features.Registration.Commands;
using Oscar.Infrastructure.Features.Registration.Queries;

namespace Oscar.Blazor.Library.Components.Works
{
    public partial class Registrations
    {
        private bool _openRegistrationsDrawer;
        private RegistrationDisplayDto _registration { get; set; }

        [Parameter]
        public Expression<Func<ICollection<RegistrationDisplayDto>>> For { get; set; }

        [Parameter]
        public ICollection<RegistrationDisplayDto> Value { get; set; }

        [Parameter]
        public int WorksId { get; set; }

        [Parameter]
        public int? ClientId { get; set; }

        [Parameter]
        public String Class { get; set; } = "";

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            return base.OnAfterRenderAsync(firstRender);
        }

        private async Task DeleteRegistrationAsync(RegistrationDisplayDto registration)
        {
            var dialog = await DialogService.ShowAsync<ConfirmDialog>("Delete Registration?");
            var dialogResult = await dialog.Result;

            if (!dialogResult.Canceled)
            {
                var result = await Mediator.Send(new DeleteRegistrationCommand { Id = registration.Id });
                if (result.IsSuccess)
                {
                    var queryResult = await Mediator.Send(new GetRegistrationsByWorksIdQuery { WorksId = WorksId });
                    if (queryResult.IsSuccess)
                    {
                        Value = queryResult.Value;
                        StateHasChanged();
                        Snackbar.Add("Registration deleted", Severity.Success);
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

        protected async Task RefreshRegistrations()
        {
            var queryResult = await Mediator.Send(new GetRegistrationsByWorksIdQuery { WorksId = WorksId });
            if (queryResult.IsSuccess)
            {
                Value = queryResult.Value;
                StateHasChanged();
            }
        }

        protected async Task OpenDrawer(RegistrationDisplayDto? registration)
        {
            if (registration != null)
                _registration = registration;
            else
                _registration = new RegistrationDisplayDto();

            _openRegistrationsDrawer = true;
            await Task.CompletedTask;
        }

        protected async Task ToggleDrawer(bool open)
        {
            _openRegistrationsDrawer = open;
            await Task.CompletedTask;
        }
    }
}
