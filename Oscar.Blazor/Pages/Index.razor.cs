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

        protected record StatTile(string Label, string Icon, Color Color, int Count);

        protected IEnumerable<StatTile> Tiles => new[]
        {
            new StatTile("Standalone", Icons.Material.Rounded.Movie, Color.Primary, standAloneCount),
            new StatTile("Series", Icons.Material.Rounded.AutoAwesomeMotion, Color.Secondary, seriesCount),
            new StatTile("Seasons", Icons.Material.Rounded.CalendarViewMonth, Color.Tertiary, seasonCount),
            new StatTile("Episodes", Icons.Material.Rounded.Theaters, Color.Warning, episodeCount)
        };

        protected static string TileIconStyle(Color color) =>
            $"background-color: rgba(var(--mud-palette-{ColorVariable(color)}-rgb), 0.12);";

        private static string ColorVariable(Color color) => color switch
        {
            Color.Primary => "primary",
            Color.Secondary => "secondary",
            Color.Tertiary => "tertiary",
            Color.Warning => "warning",
            Color.Success => "success",
            Color.Error => "error",
            _ => "info"
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
