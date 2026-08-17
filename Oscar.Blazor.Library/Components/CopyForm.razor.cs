using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Infrastructure.Features.Catalogue.Queries;
using Oscar.Infrastructure.Features.Clients.Queries;
using Oscar.Infrastructure.Features.Episode.Commands;
using Oscar.Infrastructure.Features.Episode.Queries;
using Oscar.Infrastructure.Features.Season.Commands;
using Oscar.Infrastructure.Features.Season.Queries;
using Oscar.Infrastructure.Features.Series.Commands;
using Oscar.Infrastructure.Features.Series.Queries;
using Oscar.Infrastructure.Features.Works.Commands;
using Oscar.Infrastructure.Features.Works.Queries;

namespace Oscar.Blazor.Library.Components
{
    public partial class CopyForm
    {
        private IEnumerable<ClientDto> _clients = new List<ClientDto>();
        private IEnumerable<CatalogueDto> _catalogues = new List<CatalogueDto>();
        private IEnumerable<WorksTitleDto> _seriesWorksTitle = new List<WorksTitleDto>();
        private IEnumerable<WorksTitleDto> _seasonWorksTitle = new List<WorksTitleDto>();

        private MudTable<WorksDto> table;

        private ClientDto? selectedClient;
        private CatalogueDto? selectedCatalogue;
        private WorksTitleDto? selectedSeries;
        private WorksTitleDto? selectedSeason;

        private string ActionText;
        private MudCheckBox<bool>? copyOrMoveUnderlyingWorks;

        [Parameter]
        public HashSet<WorksDto>? WorksToCopy { get; set; }

        [Parameter]
        public WorksSource WorksSource { get; set; }

        [Parameter]
        public EventCallback<CopyFormResponse> CopyFormComplete { get; set; }

        //protected override void OnInitialized()
        //{
        //    LoadClients();
        //}

        public class CopyFormResponse
        {
            public bool Response { get; set; }
            public ClientDto Client { get; set; }
            public CatalogueDto Catalogue { get; set; }
            public bool Relinquish { get; set; }
        }

        internal async Task CompleteForm(CopyFormResponse response)
        {
            await CopyFormComplete.InvokeAsync(response);
        }

        private bool IsSeriesVisible()
        {
            if (WorksSource == WorksSource.WorksDetail)
            {
                if (WorksToCopy?.FirstOrDefault()?.Discriminator == Discriminator.Episode.ToString() ||
                    WorksToCopy?.FirstOrDefault()?.Discriminator == Discriminator.Season.ToString())
                    return true;
                else
                    return false;
            }
            return false;
        }

        private bool IsSeasonsVisible()
        {
            if (WorksSource == WorksSource.WorksDetail)
            {
                if (WorksToCopy?.FirstOrDefault()?.Discriminator == Discriminator.Episode.ToString())
                    return true;
                else
                    return false;
            }
            return false;
        }

        private bool DisableCopyMove()
        {
            if (IsSeriesVisible() && selectedSeries == null)
                return true;
            if (IsSeasonsVisible() && selectedSeason == null)
                return true;

            return false;
        }

        private async Task LoadClients()
        {
            if (_clients != null && _clients?.Count() > 0) return;
            _clients = await RefDataService.GetClients();
        }

        private async Task<IEnumerable<ClientDto>> Search(string value)
        {
            await LoadClients();
            if (string.IsNullOrEmpty(value)) return _clients;

            var filteredClients = _clients.Where(x => x.ClientName.Contains(value, StringComparison.InvariantCultureIgnoreCase));

            return filteredClients;
        }

        private async Task<IEnumerable<CatalogueDto>> SearchCat(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                if (selectedCatalogue != null)
                {
                    selectedCatalogue = null;
                    StateHasChanged();
                }

                return _catalogues;
            }

            return _catalogues.Where(x => x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase));
        }

        private async Task<IEnumerable<WorksTitleDto>> SearchSeries(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                if (selectedSeries != null)
                {
                    selectedSeries = null;
                    StateHasChanged();
                }

                return _seriesWorksTitle;
            }

            return _seriesWorksTitle.Where(x => x.Title.Contains(value, StringComparison.InvariantCultureIgnoreCase));
        }

        private async Task<IEnumerable<WorksTitleDto>> SearchSeason(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                if (selectedSeason != null)
                {
                    selectedSeason = null;
                    StateHasChanged();
                }

                return _seasonWorksTitle;
            }

            return _seasonWorksTitle.Where(x => x.Title.Contains(value, StringComparison.InvariantCultureIgnoreCase));
        }

        public async Task onClientChange(EventArgs args)
        {
            if (selectedClient != null)
            {
                selectedCatalogue = null;
                var clientid = selectedClient.Id;
                _catalogues = (await Mediator.Send(new GetCataloguesQuery { ClientID = clientid })).Value;
                StateHasChanged();
            }
        }
        
        public void onCatalogueChange(EventArgs args)
        {
            if (selectedCatalogue != null)
            {
                selectedSeries = null;
                var catId = selectedCatalogue.Id;
                _seriesWorksTitle = (Mediator.Send(new GetSeriesTitlesByCatalogueIdQuery { CatalogueId = catId })).Result.Value;
                StateHasChanged();
            }
        }

        public void onSeasonChange(EventArgs args)
        {
            if (selectedSeries != null)
            {
                selectedSeason = null;
                var seriesId = selectedSeries.Id;
                _seasonWorksTitle = (Mediator.Send(new GetSeasonTitlesBySeriesIdQuery { SeriesId = seriesId })).Result.Value;
                StateHasChanged();
            }
        }

        public void cancelOnClick(MouseEventArgs args)
        {
            CompleteForm(new CopyFormResponse { Response = false, Client = selectedClient, Catalogue = selectedCatalogue });
        }

        async Task MoveWorks(bool relinquish)
        {
            var actionString = "Move";

            if (!relinquish)
                actionString = "Copy";

            ActionText = actionString + " " + table.Items.Count() + " " + "Work(s) to " + selectedClient.ClientName;

            if (selectedCatalogue != null)
                ActionText += "/" + selectedCatalogue.Name;

            var result = await DialogService.Show<ConfirmDialog>(actionString + " Works").Result;

            if (!result.Cancelled)
            {
                if (WorksSource == WorksSource.WorksDetail)
                {
                    if (WorksToCopy?.FirstOrDefault()?.Discriminator == Discriminator.Series.ToString())
                    {
                        var selectedSeriesId = WorksToCopy.FirstOrDefault().Id;
                        CopySeries(relinquish, selectedSeriesId, selectedClient.Id, selectedCatalogue.Id, copyOrMoveUnderlyingWorks.Checked);
                    }
                    else if (WorksToCopy?.FirstOrDefault()?.Discriminator == Discriminator.Season.ToString())
                    {
                        var selectedSeasonId = WorksToCopy.FirstOrDefault().Id;
                        CopySeasons(relinquish, selectedSeries.Id, selectedSeasonId, selectedClient.Id, selectedCatalogue.Id, copyOrMoveUnderlyingWorks.Checked);
                    }
                    else if (WorksToCopy?.FirstOrDefault()?.Discriminator == Discriminator.Episode.ToString())
                    {
                        var selectedEpisodeId = WorksToCopy.FirstOrDefault().Id;
                        CopyEpisode(relinquish, selectedSeries.Id, selectedSeason.Id, selectedEpisodeId, selectedClient.Id, selectedCatalogue.Id);
                    }
                    CompleteForm(new CopyFormResponse { Response = true, Client = selectedClient, Catalogue = selectedCatalogue, Relinquish = relinquish });
                }
                else if (WorksSource == WorksSource.WorksSearch)
                {
                    var selectedClientId = selectedClient.Id;
                    var selectedCatalogueId = selectedCatalogue.Id;
                    foreach (var work in table.Items.ToList())
                    {
                        
                        var workItem = await Mediator.Send(new GetWorksByIdQuery { Id = work.Id });

                        if (workItem.Value.Discriminator == "StandAlone")
                        {
                            CopyStandAlone(relinquish, work.Id, selectedClientId, selectedCatalogueId);
                        }
                        else if (workItem.Value.Discriminator == "Series")
                        {
                            CopySeries(relinquish, work.Id, selectedClientId, selectedCatalogueId, copyOrMoveUnderlyingWorks.Checked);
                        }
                        else if (workItem.Value.Discriminator == "Season")
                        {
                            var season = await Mediator.Send(new GetSeasonByIdQuery { Id = work.Id }); 
                            CopySeasons(relinquish, season.Value.Series.Id, work.Id, selectedClientId, selectedCatalogueId, copyOrMoveUnderlyingWorks.Checked);
                        }
                        else if (workItem.Value.Discriminator == "Episode")
                        {
                            var episode = await Mediator.Send(new GetEpisodeByIdQuery { Id = work.Id });
                            CopyEpisode(relinquish, episode.Value.Series.Id, episode.Value.Season.Id, work.Id, selectedClientId, selectedCatalogueId);
                        }
                    }
                    CompleteForm(new CopyFormResponse { Response = true, Client = selectedClient, Catalogue = selectedCatalogue, Relinquish = relinquish });
                }
            }
        }

        private void CopySeries(bool relinquish, int selectedSeriesId, int selectedClientId, int selectedCatalogueId, bool copyOrMoveUnderlyingWorks)
        {
            var response = (Mediator.Send(new CopySeriesCommand
            {
                Relinquish = relinquish,
                Id = selectedSeriesId,
                NewClientID = selectedClientId,
                NewCatalogueID = selectedCatalogueId,
                CopyOrMoveUnderlyingWorks = copyOrMoveUnderlyingWorks,
            })).Result.Value;

        }

        private void CopySeasons(bool relinquish, int selectedSeriesId, int selectedSeasonId, int selectedClientId, int selectedCatalogueId, bool copyOrMoveUnderlyingWorks)
        {
            var response = (Mediator.Send(new CopySeasonCommand
            {
                Relinquish = relinquish,
                NewSeriesID = selectedSeriesId,
                Id = selectedSeasonId,
                NewClientID = selectedClientId,
                NewCatalogueID = selectedCatalogueId,
                CopyOrMoveUnderlyingWorks = copyOrMoveUnderlyingWorks,
            })).Result.Value;
        }

        private void CopyEpisode(bool relinquish, int selectedSeriesId, int selectedSeasonId, int selectedEpisodeId, int selectedClientId, int selectedCatalogueId)
        {
            var response = (Mediator.Send(new CopyEpisodeCommand
            {
                Relinquish = relinquish,
                NewSeriesID = selectedSeriesId,
                NewSeasonID = selectedSeasonId,
                Id = selectedEpisodeId,
                NewClientID = selectedClientId,
                NewCatalogueID = selectedCatalogueId,
            })).Result.Value;
        }

        private void CopyStandAlone(bool relinquish, int workId, int selectedClientId, int selectedCatalogueId)
        {
            var response = (Mediator.Send(new CopyStandAloneCommand
            {
                Relinquish = relinquish,
                Id = workId,
                NewClientID = selectedClientId,
                NewCatalogueID = selectedCatalogueId,
            })).Result.Value;
        }

    }
}