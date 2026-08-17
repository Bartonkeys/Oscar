using System.Linq.Expressions;
using System.Windows.Markup;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json.Linq;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;
using Oscar.Core.Enums;
using Oscar.Infrastructure.Features.Clients.Queries;
using Oscar.Infrastructure.Features.Works.Queries;
using Severity = MudBlazor.Severity;

namespace Oscar.Blazor.Library.Components.Works
{
    public partial class Clients
    {
        [Parameter]
        public Expression<Func<ICollection<ClientReferenceDto>>> For { get; set; }

        [Parameter]
        public ICollection<ClientReferenceDto> Value { get; set; }

        private List<ClientReferenceDto> _clients;

        [Parameter]
        public EventCallback<ICollection<ClientReferenceDto>> ValueChanged { get; set; }

        public async void changeClient(ClientReferenceDto newClient, ClientReferenceDto oldClientRef)
        {
            ClientReferenceDto obj = Value.FirstOrDefault(x => x.ClientId == oldClientRef.ClientId);
            if (obj != null)
            {
                obj.ClientId = newClient.ClientId;
                obj.ClientName = newClient.ClientName;
            }
        }

        private async Task<IEnumerable<ClientReferenceDto>> Search(string value)
        {
            if (string.IsNullOrEmpty(value))
                return _clients;

            var filteredClients = _clients.Where(x => x.ClientName.Contains(value, StringComparison.InvariantCultureIgnoreCase));

            return filteredClients;
        }

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            return base.OnAfterRenderAsync(firstRender);
        }

        protected override async Task OnInitializedAsync()
        {
            _clients = new();

            List<ClientBasicDto> allclients = (await Mediator.Send(new GetClientBasicQuery { })).Value.OrderBy(x => x.ClientName).ToList();
            foreach (ClientBasicDto client in allclients)
            {
                _clients.Add(new ClientReferenceDto { ClientId = client.Id, ClientName = client.ClientName });
            }

            StateHasChanged();
        }

        private async Task addClient()
        {
            if (Value.Count() > 0)
            {
                Snackbar.Add("Only one Owning Client permitted. Please delete the current owning client before adding a new one.", Severity.Error);
                return;
            }

            ClientReferenceDto firstClient = _clients.First();
            if (Value == null) Value = new List<ClientReferenceDto>();
            Value.Add(new ClientReferenceDto { ClientId = firstClient.ClientId, ClientName = firstClient.ClientName });

            await ValueChanged.InvokeAsync(Value);

            StateHasChanged();
        }

        private async Task removeClient(ClientReferenceDto client)
        {
            Value.Remove(client);
            ValueChanged.InvokeAsync(Value);
            await Task.CompletedTask;
            StateHasChanged();
            
        }
    }
}
