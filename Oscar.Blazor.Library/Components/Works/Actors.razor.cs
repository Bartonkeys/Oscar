using FluentValidation;
using Microsoft.AspNetCore.Components;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;
using Oscar.Infrastructure.Features.Actor.Commands;
using Oscar.Infrastructure.Features.Actor.Queries;
using Severity = MudBlazor.Severity;

namespace Oscar.Blazor.Library.Components.Works
{
    public partial class Actors
    {
        private List<ActorDto> _actors;
        private ActorDto _actor;
        private String _firstName;
        private String _lastName;

        [Parameter]
        public String Class { get; set; } = "";

        [Parameter]
        public ICollection<ActorDto> Value { get; set; }

        [Parameter]
        public EventCallback<ICollection<ActorDto>> ValueChanged { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadActors();
            StateHasChanged();
        }

        private async Task LoadActors()
        {
            var people = (await Mediator.Send(new GetAllActorsQuery())).Value.OrderBy(x => x.LastName).ToList();
            _actors = people.Select(a => new ActorDto
            {
                FirstName = a.FirstName,
                LastName = a.LastName,
                Id = a.Id
            }).ToList();
        }

        private async Task<IEnumerable<ActorDto>> Search(string value)
        {
            if (string.IsNullOrEmpty(value))
                return _actors;

            var filteredActors = _actors.Where(x =>
            x.FirstName.Contains(value, StringComparison.InvariantCultureIgnoreCase)
            || x.LastName.Contains(value, StringComparison.InvariantCultureIgnoreCase));

            return filteredActors;
        }

        private async void AddActor()
        {
            Value.Add(_actor);
        }

        private async void RemoveActor(ActorDto actor)
        {
            Value.Remove(actor);
        }

        private async void CreateActor()
        {
            var addActorCommand = new AddPersonCommand<Actor>()
            {
                FirstName = _firstName,
                LastName = _lastName
            };
            var result = await Mediator.Send(addActorCommand);
            if (result.IsSuccess)
            {
                Value.Add(new ActorDto
                {
                    Id = result.Value.Id,
                    FirstName = result.Value.FirstName,
                    LastName = result.Value.LastName
                });
                StateHasChanged();
                Snackbar.Add("Actor successfully created", Severity.Success);
            }
            else
            {
                Snackbar.Add(result.Error, Severity.Error, config => { config.VisibleStateDuration = 9000; });
            }
        }

        public void onChange(EventArgs args)
        {
            
        }
    }
}
