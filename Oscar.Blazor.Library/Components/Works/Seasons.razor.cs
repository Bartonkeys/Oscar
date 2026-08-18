using System.Linq.Expressions;
using BartonKeys.Functional;
using Microsoft.AspNetCore.Components;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Infrastructure.Features.Episode.Commands;
using Oscar.Infrastructure.Features.Season.Commands;
using Oscar.Infrastructure.Features.Series.Queries;
using Severity = MudBlazor.Severity;

namespace Oscar.Blazor.Library.Components.Works
{
    public partial class Seasons
    {
        private bool _openWorksDrawer;
        private int _seasonId;

        [Parameter]
        public Expression<Func<ICollection<SeasonDto>>> For { get; set; }

        [Parameter]
        public ICollection<SeasonDto> Value { get; set; }

        [Parameter]
        public int SeriesId { get; set; }

        [Parameter]
        public String Class { get; set; } = "";

        [Parameter]
        public EventCallback<ICollection<SeasonDto>> ValueChanged { get; set; }

        [Parameter]
        public EventCallback<bool> RefreshParent { get; set; }

        [Parameter]
        public Action<int, Discriminator, string> OpenParent { get; set; }

        protected async void refreshParent()
        {
            await RefreshParent.InvokeAsync(true);
        }

        private async void removeSeason(SeasonDto season)
        {
            var dialog = await DialogService.ShowAsync<ConfirmDialog>("Remove season");
            var dialogResult = await dialog.Result;

            if (!dialogResult.Canceled)
            {
                var deleteSeasonCommand = new DeleteSeasonCommand
                {
                    Id = season.Id,
                };

                var result = await Mediator.Send(deleteSeasonCommand);
                await HandleResult(result);
                await RefreshParent.InvokeAsync(true);
            }
        }

        private async Task HandleResult<T>(Result<T> result)
        {
            if (result.IsSuccess)
            {
                Snackbar.Add("Season deleted", Severity.Success);
            }
            else
            {
                Snackbar.Add(result.Error, Severity.Error, config => { config.VisibleStateDuration = 9000; });
            }
        }

        private void OpenWorksDrawer(int id)
        {
            _openWorksDrawer = true;
            _seasonId = id;
        }

        private async void copySeason(SeasonDto season)
        {
            var dialog = await DialogService.ShowAsync<ConfirmDialog>("Copy season");
            var dialogResult = await dialog.Result;

            if (!dialogResult.Canceled)
            {
                var copySeasonCommand = new CopySeasonCommand
                {
                    Id = season.Id,
                };

                var result = await Mediator.Send(copySeasonCommand);
                await HandleCopyResult(result);
                await RefreshParent.InvokeAsync(true);
            }
        }

        private async Task HandleCopyResult<T>(Result<T> result)
        {
            if (result.IsSuccess)
            {
                Snackbar.Add("Season copied", Severity.Success);
            }
            else
            {
                Snackbar.Add(result.Error, Severity.Error, config => { config.VisibleStateDuration = 9000; });
            }
        }
    }
}
