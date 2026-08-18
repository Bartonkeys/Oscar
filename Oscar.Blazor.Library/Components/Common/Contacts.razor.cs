using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.Formatters;
using MudBlazor;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;
using Oscar.Infrastructure.Contacts.Queries;
using Oscar.Infrastructure.Features.Actor.Commands;
using Oscar.Infrastructure.Features.Actor.Queries;
using Severity = MudBlazor.Severity;

namespace Oscar.Blazor.Library.Components.Common
{
    public partial class Contacts
    {
        MudForm contactForm;
        private ContactDto contact = new();
        private ContactDto newContact = new();
        private bool openContactDrawer;
        private bool openCreateContactDrawer;
        private bool openViewDrawer;
        private bool _processing;

        //[Parameter]
        //public IEnumerable<ContactDto> ExistingContacts { get; set; } = new List<ContactDto>();

        [Parameter]
        public ICollection<ContactDto> Value { get; set; }

        //[Parameter]
        //public SocietyDto Parent { get; set; }

        //[Parameter]
        //public EventCallback<ICollection<ContactDto>> ValueChanged { get; set; }

        //private async void AddContact()
        //{
        //    Value.Add(contact);
        //    StateHasChanged();
        //    Snackbar.Add("Contact successfully added", Severity.Success);

        //    openCreateContactDrawer = false;
        //    await ValueChanged.InvokeAsync();
        //}

        private async void RemoveContact(ContactDto contact)
        {
            Value.Remove(contact);

            //await ValueChanged.InvokeAsync();
        }

        private async void CreateContact()
        {
            await contactForm.Validate();
            if (!contactForm.IsValid) return;

            Value.Add(newContact);
            StateHasChanged();
            //Snackbar.Add("Contact successfully created", Severity.Success);

            openCreateContactDrawer = false;
            newContact = new();
            //await ValueChanged.InvokeAsync();
        }

        private async Task UpdateContact()
        {
            openContactDrawer = false;
            //await ValueChanged.InvokeAsync();
        }

        private async Task EditContact(ContactDto context)
        {
            contact = context;
            openContactDrawer = true;
        }

        private void ViewContact(ContactDto context)
        {
            contact = context;
            openViewDrawer = true;
        }

        //private async Task<IEnumerable<ContactDto>> SearchContact(string value, CancellationToken token)
        //{
        //    if (string.IsNullOrEmpty(value))
        //        return ExistingContacts;

        //    var filteredContacts = ExistingContacts.Where(x =>
        //        (x.FirstName != null &&  x.FirstName.Contains(value, StringComparison.InvariantCultureIgnoreCase))
        //        || (x.LastName != null && x.LastName.Contains(value, StringComparison.InvariantCultureIgnoreCase)));

        //    return filteredContacts;
        //}
    }
}
