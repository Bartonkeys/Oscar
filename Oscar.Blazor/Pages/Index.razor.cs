using Microsoft.AspNetCore.Components;
using MudBlazor;
using NuGet.Protocol.Plugins;
using Oscar.Data.Context;

namespace Oscar.Blazor.Pages
{
    public partial class Index
    {
        [Inject] private OscarContext OscarContext { get; set; }

        private static int standAloneCount;
        private static int seriesCount;
        private static int seasonCount;
        private static int episodeCount;
        private bool loading = true;
        public double[] data = { standAloneCount, seriesCount, seasonCount, episodeCount };
        public string[] labels = { "Standalone", "Series", "Season", "Episode" };

        protected override async Task OnInitializedAsync() => await Task.Run(LoadClients);

        private async Task LoadClients()
        {
            standAloneCount = OscarContext.Works.Count(w => w.Discriminator == "StandAlone");
            seriesCount = OscarContext.Works.Count(w => w.Discriminator == "Series");
            seasonCount = OscarContext.Works.Count(w => w.Discriminator == "Season");
            episodeCount = OscarContext.Works.Count(w => w.Discriminator == "Episode");
            loading = false;
            await Task.CompletedTask;
        }
    }
}