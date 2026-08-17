using Microsoft.AspNetCore.Components;

using MudBlazor;
using Oscar.Blazor.Library.Common;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Infrastructure.Features.Works.Queries;

namespace Oscar.Blazor.Pages
{
    public partial class WorksDetail: OscarComponentBase
    {
        [Parameter]
        public int Id { get; set; }

        [Parameter]
        public string Discriminator { get; set; }

        private Discriminator _discriminator;

        public List<BreadcrumbItem> Breadcrumbs { get; set; } = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("WORKS SEARCH", href: "works", icon: Icons.Material.Filled.Search)
        };
        private List<CountryDto> _countries;
        //private List<PersonDto> _actors;
        private List<RightsTypeDto> _rightsType;

        protected override async void OnInitialized()
        {
            await LoadReferenceData();
        }

        protected override void OnAfterRender(bool firstRender)
        {
        }

        protected override async Task OnParametersSetAsync()
        {
            _discriminator = Enum.Parse<Discriminator>(Discriminator);

            if (Id == 0) return;

            if (Breadcrumbs.Any(b => b.Href.Contains(Id.ToString()))) return;

            var worksTitle = (await Mediator.Send(new GetWorksTitleQuery { Id = Id })).Value?.Title ?? string.Empty;
            Breadcrumbs.Add(new BreadcrumbItem($"{_discriminator.ToString().ToUpper()}: {worksTitle.ToUpper()}", href: $"worksDetail/{_discriminator}/{Id}", icon: GetIcon()));
        }

        private async Task LoadReferenceData()
        {
            //if ((_actors?.Count ?? 0) == 0)
            //{
            //    var actors = await RefDataService.GetActors();
            //    _actors = actors.OrderBy(x => x.LastName).ToList();
            //}

            if ((_rightsType?.Count ?? 0) == 0)
            {
                _rightsType = await RefDataService.GetRightsType();
            }

            if ((_countries?.Count ?? 0) == 0)
            {
                _countries = await RefDataService.GetCountries();
            }
        }

        protected void OpenWorksDetail(int id, Discriminator discriminator, string title)
        {
            NavigationManager.NavigateTo($"worksDetail/{discriminator}/{id}");
        }

        private string GetIcon()
        {
            return _discriminator switch
            {
                Core.Enums.Discriminator.Episode => Icons.Material.Filled.Theaters,
                Core.Enums.Discriminator.Season => Icons.Material.Filled.WbSunny,
                Core.Enums.Discriminator.Series => Icons.Material.Filled.AutoAwesomeMotion,
                Core.Enums.Discriminator.StandAlone => Icons.Material.Filled.Man,
                _ => string.Empty
            };
        }
    }
}
