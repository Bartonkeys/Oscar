using Microsoft.AspNetCore.Components;
using MudBlazor;
using Oscar.Data.Context;

namespace Oscar.Blazor.Pages
{
    public partial class Index
    {
        [Inject] private OscarContext OscarContext { get; set; }

        private int standAloneCount;
        private int seriesCount;
        private int seasonCount;
        private int episodeCount;
        private bool loading = true;

        public double[] data => new double[] { standAloneCount, seriesCount, seasonCount, episodeCount };
        public string[] labels = { "Standalone", "Series", "Season", "Episode" };

        public List<ChartSeries<double>> Series => new()
        {
            new ChartSeries<double> { Name = "Works", Data = data }
        };

        protected override async Task OnInitializedAsync() => await LoadWorkCounts();

        private async Task LoadWorkCounts()
        {
            standAloneCount = await CountByDiscriminator("StandAlone");
            seriesCount = await CountByDiscriminator("Series");
            seasonCount = await CountByDiscriminator("Season");
            episodeCount = await CountByDiscriminator("Episode");
            loading = false;
        }

        private Task<int> CountByDiscriminator(string discriminator) =>
            Task.FromResult(OscarContext.Works.Count(w => w.Discriminator == discriminator));
    }
}
