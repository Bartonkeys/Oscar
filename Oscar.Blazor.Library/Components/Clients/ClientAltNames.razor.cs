using Microsoft.AspNetCore.Components;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;

namespace Oscar.Blazor.Library.Components.Clients
{
    public partial class ClientAltNames
    {
        private string? _newName;
        private AltNameType? _newAltNameType;

        [Parameter]
        public ICollection<ClientAltNameDto> Value { get; set; }

        [Parameter]
        public EventCallback<ICollection<ClientAltNameDto>> ValueChanged { get; set; }

        private void AddClientAltName()
        {
            if (_newName != null)
            {
                Value.Add(new ClientAltNameDto
                {
                    AltName = _newName,
                    AltNameType = _newAltNameType ?? AltNameType.AKA
                });
            }
        }

        private async Task RemoveClientAltName(ClientAltNameDto clientAltName)
        {
            Value.Remove(clientAltName);
            await Task.CompletedTask;
        }
    }
}

