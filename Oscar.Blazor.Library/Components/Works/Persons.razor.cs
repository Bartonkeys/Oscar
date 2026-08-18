using System.Linq;
using BartonKeys.Functional;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;
using Oscar.Core.Enums;
using Oscar.Infrastructure.Features.Actor.Commands;
using Oscar.Infrastructure.Features.Actor.Queries;
using Oscar.Infrastructure.Features.Director.Queries;
using Oscar.Infrastructure.Features.Distributor.Queries;
using Oscar.Infrastructure.Features.Producer.Queries;
using Oscar.Infrastructure.Features.ScreenWriter.Queries;
using Oscar.Infrastructure.Features.ScriptWriter.Queries;
using Severity = MudBlazor.Severity;

namespace Oscar.Blazor.Library.Components.Works
{
    public partial class Persons
    {
        private PersonDto _person;
        private String _firstName;
        private String _lastName;
        private String _lastNameLabel = "Last Name";

        [Parameter]
        public String Class { get; set; } = "";

        [Parameter]
        public String Header { get; set; } = "";

        [Parameter]
        public String ListLabel { get; set; } = "";

        [Parameter]
        public String CreateLabel { get; set; } = "";

        [Parameter]
        public ICollection<PersonDto> Value { get; set; }

        [Parameter]
        public EventCallback<ICollection<PersonDto>> ValueChanged { get; set; }

        [Parameter]
        public PersonType type { get; set; }

        //[Parameter]
        public List<PersonDto> _persons { get; set; }



        protected override async Task OnParametersSetAsync()
        {
            if (type == PersonType.Distributor)
            {
                _lastNameLabel = "Name";
            }

        }

        private async Task LoadPersons()
        {
            switch (type)
            {
                case PersonType.Actor: 
                    _persons = await RefDataService.GetActors(); 
                    break;
                case PersonType.Director: 
                    _persons = await RefDataService.GetDirectors(); 
                    break;
                case PersonType.Producer: 
                    _persons = await RefDataService.GetProducers(); 
                    break;
                case PersonType.Distributor:
                    _persons = await RefDataService.GetDistributors(); 
                    break;
                case PersonType.ScreenWriter:
                    _persons = await RefDataService.GetScreenWriters(); 
                    break;
                case PersonType.ScriptWriter: 
                    _persons = await RefDataService.GetScriptWriters();
                    break;
            }
            _persons.RemoveAll(x => Value.Select(c => c.Id).ToList().Contains(x.Id));
        }

        private async Task<IEnumerable<PersonDto>> Search(string value, CancellationToken token)
        {
            if(_persons == null) 
                await LoadPersons();

            if (String.IsNullOrWhiteSpace(value))
                return _persons;

            string[] names = value.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (names.Length > 1)
            {
                return _persons.Where(x => x.FirstName.Contains(names[0].Trim(), StringComparison.InvariantCultureIgnoreCase)
                && x.LastName.Contains(names[1].Trim(), StringComparison.InvariantCultureIgnoreCase));
            }
            return _persons.Where(x => x.FirstName.Contains(names[0].Trim(), StringComparison.InvariantCultureIgnoreCase)
            || x.LastName.Contains(names[0].Trim(), StringComparison.InvariantCultureIgnoreCase));
        }

        private async void AddPerson()
        {
            if (_person != null && !Value.Contains(_person))
            {
                Value.Add(_person);
                if (_persons != null) 
                    _persons.Remove(_person);
            }
        }

        private async void RemovePerson(PersonDto person)
        {
            Value.Remove(person);
            if(_persons != null)
                _persons.Add(person);
        }

        private async void CreatePerson()
        {
            switch (type)
            {
                case PersonType.Actor:
                    var addActorCommand = new AddPersonCommand<Actor>()
                    {
                        FirstName = _firstName,
                        LastName = _lastName
                    };
                    var resultActor = await Mediator.Send(addActorCommand);
                    await HandleResult<PersonDto>(resultActor);
                    break;

                case PersonType.Director:
                    var addDirectorCommand = new AddPersonCommand<Director>()
                    {
                        FirstName = _firstName,
                        LastName = _lastName
                    };
                    var resultDirector = await Mediator.Send(addDirectorCommand);
                    await HandleResult<PersonDto>(resultDirector);
                    break;

                case PersonType.Producer:
                    var addProducerCommand = new AddPersonCommand<Producer>()
                    {
                        FirstName = _firstName,
                        LastName = _lastName
                    };
                    var resultProducer = await Mediator.Send(addProducerCommand);
                    await HandleResult<PersonDto>(resultProducer);
                    break;

                case PersonType.Distributor:
                    var addDistributorCommand = new AddPersonCommand<Distributor>()
                    {
                        FirstName = "Distributor",
                        LastName = _lastName
                    };
                    var resultDistributor = await Mediator.Send(addDistributorCommand);
                    await HandleResult<PersonDto>(resultDistributor);
                    break;

                case PersonType.ScreenWriter:
                    var addScreenWriterCommand = new AddPersonCommand<ScreenWriter>()
                    {
                        FirstName = _firstName,
                        LastName = _lastName
                    };
                    var resultScreenWriter = await Mediator.Send(addScreenWriterCommand);
                    await HandleResult<PersonDto>(resultScreenWriter);
                    break;

                case PersonType.ScriptWriter:
                    var addScriptWriterCommand = new AddPersonCommand<ScriptWriter>()
                    {
                        FirstName = _firstName,
                        LastName = _lastName
                    };
                    var resultScriptWriter = await Mediator.Send(addScriptWriterCommand);
                    await HandleResult<PersonDto>(resultScriptWriter);
                    break;
            }
        }

        private async Task HandleResult<T>(Result<PersonDto> result) where T : PersonDto
        {
            if (result.IsSuccess)
            {
                Value.Add(result.Value);
                StateHasChanged();
                Snackbar.Add("Successfully created", Severity.Success);
            }
            else
            {
                Snackbar.Add(result.Error, Severity.Error, config => { config.VisibleStateDuration = 9000; });
            }
        }

        private String? ListItemString(PersonDto? person)
        {
            String? listItem = null;
            if(person != null)
            {
                listItem = person.LastName;
                if(type != PersonType.Distributor)
                {
                    listItem = person.FirstName + " " + listItem;
                }
            }
            return listItem;
        }

        public void onChange(EventArgs args)
        {

        }
    }
}

