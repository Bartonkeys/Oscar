using System.Linq.Expressions;
using BartonKeys.Functional;
using Microsoft.AspNetCore.Components;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;
using Oscar.Core.Enums;
using Oscar.Infrastructure.Features.Episode.Commands;
using Oscar.Infrastructure.Features.Season.Queries;
using Oscar.Infrastructure.Features.Series.Queries;
using Severity = MudBlazor.Severity;

namespace Oscar.Blazor.Library.Components.Works
{
    public partial class Episodes
    {
        private bool _openWorksDrawer;
        private bool _openEpsBulkAddDrawer;
        private int _episodeId;
        private String OwningClient = "";
        private String OwningCatalogue = "";

        [Parameter]
        public Expression<Func<ICollection<EpisodeDto>>> For { get; set; }

        [Parameter]
        public ICollection<EpisodeDto> Value { get; set; }

        [Parameter]
        public int SeasonId { get; set; }

        [Parameter]
        public int SeriesId { get; set; }

        [Parameter]
        public WorksDto ParentWork { get; set; }

        [Parameter]
        public String Class { get; set; } = "";

        [Parameter]
        public EventCallback<ICollection<EpisodeDto>> ValueChanged { get; set; }

        [Parameter]
        public EventCallback<bool> RefreshParent { get; set; }

        [Parameter]
        public Action<int, Discriminator, string> OpenParent { get; set; }

        [Parameter]
        public WorksTitleDto ParentTitle { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            await loadData();
        }

        protected async Task loadData()
        {
            await base.OnParametersSetAsync();
            SeriesDto series = (await Mediator.Send(new GetSeriesBasicByIdQuery
            {
                Id = SeriesId
            })).Value;

            if(series != null && series.Clients != null && series.Clients.Count() > 0)
            {
                OwningClient = series.Clients.First().ClientName;
            }

            if (series != null && series.Catalogues != null && series.Catalogues.Count() > 0)
            {
                OwningCatalogue = series.Catalogues.First().Name;
            }
        }

        protected async void refreshParent()
        {
            await RefreshParent.InvokeAsync(true);
        }

        private async void removeEpisode(EpisodeDto episode)
        {
            var dialog = DialogService.Show<ConfirmDialog>("Remove episode");
            var dialogResult = await dialog.Result;

            if (!dialogResult.Cancelled)
            {
                var deleteEpisodeCommand = new DeleteEpisodeCommand
                {
                    Id = episode.Id,
                };

                var result = await Mediator.Send(deleteEpisodeCommand);
                await HandleResult(result);
                await RefreshParent.InvokeAsync(true);
            }
        }

        private async void copyEpisode(EpisodeDto episode)
        {
            var dialog = DialogService.Show<ConfirmDialog>("Copy episode");
            var dialogResult = await dialog.Result;

            if (!dialogResult.Cancelled)
            {
                var copyEpisodeCommand = new CopyEpisodeCommand
                {
                    Id = episode.Id,
                };

                var result = await Mediator.Send(copyEpisodeCommand);
                await HandleCopyResult(result);
                await RefreshParent.InvokeAsync(true);
            }
        }

        private async Task HandleCopyResult<T>(Result<T> result)
        {
            if (result.IsSuccess)
            {
                Snackbar.Add("Episode copied", Severity.Success);
            }
            else
            {
                Snackbar.Add(result.Error, Severity.Error, config => { config.VisibleStateDuration = 9000; });
            }
        }

        private async Task HandleResult<T>(Result<T> result)
        {
            if (result.IsSuccess)
            {
                Snackbar.Add("Episode deleted", Severity.Success);
            }
            else
            {
                Snackbar.Add(result.Error, Severity.Error, config => { config.VisibleStateDuration = 9000; });
            }
        }

        private void OpenWorksDrawer(int id)
        {
            _openWorksDrawer = true;
            _episodeId = id;
            //NavigationManager.NavigateTo($"WorksDetail/Episode/{id}");
        }

        private void OpenEpsBulkAddDrawer()
        {
            _openEpsBulkAddDrawer = true;
        }

        protected async Task ToggleWorksDrawer(bool open)
        {
            _openWorksDrawer = open;

        }

        protected async Task ToggleEpsBulkAddDrawer(bool open)
        {
            _openEpsBulkAddDrawer = open;

        }
    }
}
