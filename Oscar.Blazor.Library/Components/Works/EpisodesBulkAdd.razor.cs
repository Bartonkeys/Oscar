using BartonKeys.Extensions;
using BartonKeys.Functional;
using Microsoft.AspNetCore.Components;
using Oscar.Blazor.Library.Components.Common;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Episode.Commands;
using Oscar.Infrastructure.Features.Episode.Queries;
using Oscar.Infrastructure.Features.Works.Queries;
using Severity = MudBlazor.Severity;

namespace Oscar.Blazor.Library.Components.Works
{

    public partial class EpisodesBulkAdd
    {
        private ICollection<TitleLanguageDto> _titles = new List<TitleLanguageDto>();
        private List<LanguageDto> _languages;
        private bool _processing = false;
        private int _maxEpisodeNumber;

        [Parameter]
        public int SeasonId { get; set; }

        [Parameter]
        public int SeriesId { get; set; }

        [Parameter]
        public WorksTitleDto ParentTitle { get; set; }

        [Parameter]
        public String Class { get; set; } = "";

        [Parameter]
        public EventCallback<bool> RefreshParent { get; set; }

        [Parameter]
        public Action<int, Discriminator, string> OpenParent { get; set; }

        [Parameter]
        public EventCallback<bool> toggleWorksDrawer { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _languages = (await Mediator.Send(new GetLanguageStaticDataQuery { })).Value;
            var episodes = (await Mediator.Send(new GetEpisodeBySeasonIdQuery { SeasonId = SeasonId }));
            _maxEpisodeNumber = episodes.Value.Max(x => x.Number) ?? 0;

            addTitle();
        }

        protected async void refreshParent()
        {
            await RefreshParent.InvokeAsync(true);
        }

        private async Task Submit()
        {
            _processing = true;
            var bulkAddEpisodeCommand = new BulkAddEpisodeCommand
            {
                SeasonId = SeasonId,
                EpisodeTitles = _titles,
                SeriesId = SeriesId
            };

            var result = await Mediator.Send(bulkAddEpisodeCommand);
            await HandleResult(result);
            await RefreshParent.InvokeAsync(true);
            _processing = false;
        }

        private async Task HandleResult<T>(Result<T> result)
        {
            if (result.IsSuccess)
            {
                Snackbar.Add("Episodes created", Severity.Success);
            }
            else
            {
                Snackbar.Add(result.Error, Severity.Error, config => { config.VisibleStateDuration = 9000; });
            }
        }

        private async Task<IEnumerable<LanguageDto>> Search(string value)
        {
            if (string.IsNullOrEmpty(value))
                return _languages;

            var filteredLanguages = _languages.Where(x => x.Description.Contains(value, StringComparison.InvariantCultureIgnoreCase));

            return filteredLanguages;
        }

        private async void addTitle()
        {
            _maxEpisodeNumber = _maxEpisodeNumber + 1;
            LanguageDto language = _languages.Find(l => l.Name == ParentTitle?.LanguageCode);
            _titles.Add(new TitleLanguageDto { EpisodeNumber = _maxEpisodeNumber, Language = language, Title = string.Empty });
            //StateHasChanged();
        }

        private async void removeTitle(TitleLanguageDto title)
        {
            if (_titles.Count() == 1)
                Snackbar.Add("At least one episode required", Severity.Error);
            else
            {
                _titles.Remove(title);
                //StateHasChanged();
            }
        }

        protected async Task Cancel()
        {
            await toggleWorksDrawer.InvokeAsync(false);
        }
    }
}
