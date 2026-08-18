using FluentValidation;
using Microsoft.AspNetCore.Components;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;
using Oscar.Infrastructure.Features.Channel.Queries;
using System.Linq;

namespace Oscar.Blazor.Library.Components.Rights
{
    public partial class Channels
    {
        private List<ChannelDto> _channels;
        private ChannelDto? _channel;

        [Parameter]
        public String Style { get; set; } = "";

        [Parameter]
        public String Class { get; set; } = "";

        [Parameter]
        public String Header { get; set; } = "";

        [Parameter]
        public String ListLabel { get; set; } = "";

        [Parameter]
        public ICollection<ChannelRightsDto> Value { get; set; }

        [Parameter]
        public EventCallback<ICollection<ChannelRightsDto>> ValueChanged { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            _channel = default;
            _channels = (await Mediator.Send(new GetAllChannelsQuery())).Value.OrderBy(x => x.Name).ToList();
            if (Value.Any())
            {
                _channels.RemoveAll(x => Value.Select(c => c.Channel.Id).ToList().Contains(x.Id));
            }

            _channel = _channels?.FirstOrDefault(x => x.Name == "*");
            StateHasChanged();
        }

        private async Task<IEnumerable<ChannelDto>> Search(string value, CancellationToken token)
        {
            if (string.IsNullOrEmpty(value))
                return _channels;

            var filteredChannels = _channels.Where(x =>
            x.Name.StartsWith(value, StringComparison.InvariantCultureIgnoreCase));

            return filteredChannels;
        }

        private void AddChannel()
        {
            if (_channel != null && !Value.Any(v => v.Channel.Id == _channel.Id))
            {
                ChannelRightsDto cr = new();
                cr.Channel = _channel;
                Value.Add(cr);
                _channels.RemoveAll(c => c.Id == _channel.Id);
                _channel = default;
            }
        }

        private void RemoveChannel(ChannelDto channel)
        {
            ChannelRightsDto channelToRemove = Value.First(v => v.Channel.Id == channel.Id);
            if (channelToRemove != null)
            {
                Value.Remove(channelToRemove);
                _channels.Add(channel);
                _channels = _channels.OrderBy(x => x.Name).ToList();
                _channel = default;
            }
        }

        public void onChange(EventArgs args)
        {

        }
    }
}

