using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Oscar.Core.DTOs;
using Oscar.Infrastructure.Contacts.Queries;
using Oscar.Infrastructure.Features.Society.Commands;
using Oscar.Infrastructure.Features.Society.Queries;

namespace Oscar.Blazor.Library.Components.Societies
{
    public partial class SocietyForm
    {
        private SocietyDto? _society = new()
        {
            Addresses = new List<AddressDto>(),
            Contacts = new List<ContactDto>()
        };
        MudForm form = new();
        private bool _processing;
        private AddressDto newAddress = new();
        //private IEnumerable<ContactDto> _existingContacts;

        [Parameter]
        public int Id { get; set; } = 0;

        [Parameter]
        public bool Open { get; set; }

        [Parameter]
        public EventCallback onSuccess { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            if (Id == 0)
            {
                form.ResetValidation();
                _society = new()
                {
                    Addresses = new List<AddressDto>(),
                    Contacts = new List<ContactDto>()
                };
                return;
            }

            var societyResult = await Mediator.Send(new GetSocietyQuery { Id = Id });

            if (societyResult.IsFailure) return;

            _society = societyResult.Value;

            //var contactsResult = await Mediator.Send(new GetAllContactsQuery());
            //if (contactsResult.IsSuccess)
            //    _existingContacts = contactsResult.Value;
        }

        private async Task Submit()
        {
            if (_society.Id == 0) return;

            if(!_society.Addresses.Any())
                _society.Addresses.Add(newAddress);

            _processing = true;
            var result = await Mediator.Send(new UpdateSocietyCommand { SocietyDto = _society });
            _processing = false;

            if (result.IsSuccess)
                Snackbar.Add("Society updated", Severity.Success);
            else
                Snackbar.Add(result.Error, Severity.Error);
        }

        private async Task Create()
        {
            await form.Validate();
            if (!form.IsValid) return;

            _processing = true;

            _society.Addresses.Add(newAddress);

            var result = await Mediator.Send(new AddSocietyCommand { SocietyDto = _society });

            _processing = false;

            if (result.IsSuccess)
                Snackbar.Add("Society created", Severity.Success);
            else
                Snackbar.Add(result.Error, Severity.Error);
        }

        private async Task OnContactAdd()
        {
            //if (_society.Id == 0) return;

            //_processing = true;
            //var result = await Mediator.Send(new UpdateSocietyCommand { SocietyDto = _society });
            //_processing = false;

            //if (result.IsSuccess)
            //    Snackbar.Add("Society updated", Severity.Success);
            //else
            //    Snackbar.Add(result.Error, Severity.Error);
        }

    }
}
