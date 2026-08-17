using Microsoft.AspNetCore.Components;
using MudBlazor;
using Oscar.Core.DTOs;
using Oscar.Infrastructure.Features.Clients.Queries;

namespace Oscar.Blazor.Pages
{
    public partial class ImportEpisode
    {
        private int totalItems;
        private string searchString = null;
        private List<ClientBasicDto> _clients;
        private bool openEpisodeCreate;

        protected override async Task OnInitializedAsync()
        {
            _clients = (await Mediator.Send(new GetClientBasicQuery())).Value;
        }

        private async Task Refresh()
        {
            openEpisodeCreate = false;
        }

        private async Task OpenEpisodeCreate()
        {
            openEpisodeCreate = true;
        }

    }
}
