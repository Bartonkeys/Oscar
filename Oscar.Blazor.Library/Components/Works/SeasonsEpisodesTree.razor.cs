using System.Linq.Expressions;
using BartonKeys.Functional;
using MediatR;
using Microsoft.AspNetCore.Components;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;
using Oscar.Core.Enums;
using Oscar.Infrastructure.Features.Episode.Commands;
using Oscar.Infrastructure.Features.Season.Commands;
using Oscar.Infrastructure.Features.Season.Queries;
using Oscar.Infrastructure.Features.Series.Queries;
using static Oscar.Blazor.Library.Components.CopyForm;
using Severity = MudBlazor.Severity;

namespace Oscar.Blazor.Library.Components.Works
{
    public partial class SeasonsEpisodesTree
    {
        private bool _openWorksDrawer;
        private int _selectedId;
        private int _seasonId;
        private Discriminator _discriminator;
        private HashSet<TreeItemData> _treeItems = new HashSet<TreeItemData>();
        private bool _loading = false;
        private HashSet<WorksDto> selectedItems = new();
        private bool openCopyDrawer;
        private string actionText = "";


        [Parameter]
        public ICollection<SeasonDto>? Seasons { get; set; }

        [Parameter]
        public int SeriesId { get; set; }

        [Parameter]
        public SeriesDto ParentSeries { get; set; }

        [Parameter]
        public String Class { get; set; } = "";

        [Parameter]
        public String OwningClient { get; set; } = "";

        [Parameter]
        public String OwningCatalogue { get; set; } = "";

        [Parameter]
        public EventCallback<bool> RefreshParent { get; set; }

        [Parameter]
        public Action<int, Discriminator, string> OpenParent { get; set; }

        protected async void refreshParent()
        {
            await RefreshParent.InvokeAsync(true);
        }

        private async void removeItem(TreeItemData item)
        {
            String message = "Remove ";
            if (item.Discriminator == Discriminator.Season)
            {
                message += "Season";
            }
            if (item.Discriminator == Discriminator.Episode)
            {
                message += "Episode";
            }
            var dialog = DialogService.Show<ConfirmDialog>(message);
            var dialogResult = await dialog.Result;

            if (!dialogResult.Cancelled)
            {
                if (item.Discriminator == Discriminator.Season)
                {
                    var deleteSeasonCommand = new DeleteSeasonCommand
                    {
                        Id = item.Id,
                    };

                    var result = await Mediator.Send(deleteSeasonCommand);
                    await HandleResult(result, item);
                    await RefreshParent.InvokeAsync(true);
                }

                if (item.Discriminator == Discriminator.Episode)
                {
                    var deleteEpisodeCommand = new DeleteEpisodeCommand
                    {
                        Id = item.Id,
                    };

                    var result = await Mediator.Send(deleteEpisodeCommand);
                    await HandleResult(result, item);
                    await RefreshParent.InvokeAsync(true);
                }
            }
        }

        private async Task HandleResult<T>(Result<T> result, TreeItemData item)
        {
            String message = "Item ";
            if (item.Discriminator == Discriminator.Season)
            {
                message = "Season";
            }
            if (item.Discriminator == Discriminator.Episode)
            {
                message = "Episode";
            }

            if (result.IsSuccess)
            {
                Snackbar.Add(message + " deleted", Severity.Success);
            }
            else
            {
                Snackbar.Add(result.Error, Severity.Error, config => { config.VisibleStateDuration = 9000; });
            }
        }

        private void OpenWorksDrawer(TreeItemData item)
        {
            _openWorksDrawer = true;
            if (item == null)
            {
                _selectedId = 0;
                _discriminator = Discriminator.Season;
            }
            else
            {
                _selectedId = item.Id;
                _discriminator = item.Discriminator;
                _seasonId = item.SeasonId;
                //NavigationManager.NavigateTo($"WorksDetail/{item.Discriminator}/{item.Id}");
            }
        }

        private void OpenCopyDrawer(TreeItemData item)
        {
            openCopyDrawer = true;
            selectedItems.Clear();
            selectedItems.Add(new WorksDto
            {
                Id = item.Id,
                Discriminator = item.Discriminator.ToString(),
                ProductionYear = item.ProductionYear,
                Titles = new List<WorksTitleDto>(){
                            new WorksTitleDto() {
                                Title = item.Title
                            }
                        }
            });
        }

        private async Task CopyFormComplete(CopyFormResponse response)
        {
            openCopyDrawer = false;

            if (response.Response)
            {
                var actionString = "Copied";

                if (response.Relinquish)
                    actionString = "Moved";

                actionText = selectedItems.Count() + " Works " + actionString + " to " + response.Client.ClientName;

                if (response.Catalogue != null)
                    actionText += "/" + response.Catalogue.Name;

                if (response.Response)
                    Snackbar.Add(actionText);

                selectedItems = new HashSet<WorksDto>();
                await RefreshParent.InvokeAsync(true);
            }
        }

        private async Task HandleCopyResult<T>(Result<T> result)
        {
            if (result.IsSuccess)
            {
                Snackbar.Add("Sucessfully copied", Severity.Success);
            }
            else
            {
                Snackbar.Add(result.Error, Severity.Error, config => { config.VisibleStateDuration = 9000; });
            }
        }

        protected async override Task OnInitializedAsync()
        {
            _loading = true;
            _treeItems.Clear();

            HashSet<int> addedEpisodeIds = new HashSet<int>();

            foreach (SeasonDto season in Seasons.OrderBy(s => s.Number))
            {
                SeasonDto s = (await Mediator.Send(new GetSeasonBasicByIdQuery
                {
                    Id = season.Id
                })).Value;

                HashSet<TreeItemData> _episodeTreeItems = new HashSet<TreeItemData>();
                foreach (EpisodeDto episode in s.Episodes.OrderBy(e => e.Number))
                {
                    string title = episode.Titles?.FirstOrDefault()?.Title ?? string.Empty;
                    if (episode.Number != null)
                    {
                        title += " (Episode " + episode.Number + ")";
                    }
                    _episodeTreeItems.Add(new TreeItemData(title, episode.Id, null, Discriminator.Episode, episode.ProductionYear, season.Id));
                    addedEpisodeIds.Add(episode.Id);
                }
                _treeItems.Add(new TreeItemData(season?.Titles.FirstOrDefault()?.Title ?? string.Empty, season.Id, _episodeTreeItems, Discriminator.Season, season.ProductionYear, 0));
            }

            HashSet<TreeItemData> standaloneEpisodeTreeItems = new HashSet<TreeItemData>();
            foreach (EpisodeDto episode in ParentSeries.Episodes.OrderBy(e => e.Number))
            {
                if (!addedEpisodeIds.Contains(episode.Id))
                {
                    string title = episode.Titles?.FirstOrDefault()?.Title ?? string.Empty;
                    if (episode.Number != null)
                    {
                        title += " (Episode " + episode.Number + ")";
                    }

                    standaloneEpisodeTreeItems.Add(new TreeItemData(title, episode.Id, null, Discriminator.Episode, episode.ProductionYear, ParentSeries.Id));
                    addedEpisodeIds.Add(episode.Id);
                }
            }

            if (standaloneEpisodeTreeItems.Count > 0)
            {
                _treeItems.Add(new TreeItemData("Episodes", 0, standaloneEpisodeTreeItems, Discriminator.All, null, ParentSeries.Id));
            }

            _loading = false;
        }

        public class TreeItemData
        {
            public string Title { get; set; }

            public HashSet<TreeItemData> TreeItems { get; set; }

            public bool HasChild => TreeItems != null && TreeItems.Count > 0;

            public bool IsExpanded { get; set; } = false;

            public int Id { get; set; }

            public Discriminator Discriminator { get; set; }

            public int? ProductionYear { get; set; }

            public int SeasonId { get; set; }

            public TreeItemData(string title, int id, HashSet<TreeItemData> treeItems, Discriminator discriminator, int? productionYear, int seasonId)
            {
                Title = title;
                Id = id;
                TreeItems = treeItems;
                Discriminator = discriminator;
                ProductionYear = productionYear;
                SeasonId = seasonId;
            }
        }

        protected async Task ToggleWorksDrawer(bool open)
        {
            _openWorksDrawer = open;

        }
    }
}
