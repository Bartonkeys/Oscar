using FluentValidation;
using Microsoft.AspNetCore.Components;
using Oscar.Core.DTOs;
using Oscar.Infrastructure.Features.Society.Queries;

namespace Oscar.Blazor.Library.Components.Works
{
    public partial class ReRegistrations
    {
        private List<SocietyDto>? _societies;
        private SocietyDto? _society;

        [Parameter]
        public String Class { get; set; } = "";

        [Parameter]
        public String Header { get; set; } = "";

        [Parameter]
        public String ListLabel { get; set; } = "";

        [Parameter]
        public String CreateLabel { get; set; } = "";

        [Parameter]
        public ICollection<ReRegistrationDto> Values { get; set; }

        [Parameter]
        public EventCallback<ICollection<ReRegistrationDto>> ValuesChanged { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadAllSocieties();
            StateHasChanged();
        }

        private async Task LoadAllSocieties()
        {
            _societies = (await Mediator.Send(new GetAllSocietiesQuery())).Value.OrderBy(x => x.Name).ToList();
            _societies.RemoveAll(x => Values.Select(c => c.Id).ToList().Contains(x.Id));
        }

        private async Task<IEnumerable<SocietyDto>> Search(string value)
        {
            if(_societies == null) 
                await LoadAllSocieties();

            if (string.IsNullOrWhiteSpace(value))
                return _societies;

            return _societies.Where(x => x.Name.Contains(value.Trim(), StringComparison.InvariantCultureIgnoreCase));
        }

        private async void AddReRegistration()
        {
            if (_society != null && !Values.Any(c => c.Society.Id == _society.Id))
            {
                var reRegistration = new ReRegistrationDto
                {
                    Society = _society
                };
                Values.Add(reRegistration);
                _societies.Remove(_society);
            }
            _societies = default;
        }

        private async void RemoveReRegistration(ReRegistrationDto reRegistrationDtoDto)
        {
            Values.Remove(reRegistrationDtoDto);

            if(_societies.All(o => o.Id != reRegistrationDtoDto.Society.Id))
                _societies.Add(reRegistrationDtoDto.Society);
        }

        private String? ListItemString(SocietyDto? society)
        {
            String? listItem = null;
            if(society != null)
            {
                listItem = society.Name;
            }
            return listItem;
        }

        private String? ListItemString(ReRegistrationDto? reRegistrationDto)
        {
            String? listItem = null;
            if (reRegistrationDto != null)
            {
                listItem = reRegistrationDto?.Society?.Name;
            }
            return listItem;
        }

        public void onChange(EventArgs args)
        {

        }
    }
}

