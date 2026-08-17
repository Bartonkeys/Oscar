using BartonKeys.Functional;
using Microsoft.AspNetCore.Components;
using Oscar.Core.DTOs;
using Severity = MudBlazor.Severity;
using FluentValidation;
using MudBlazor;
using Oscar.Core.Enums;
using Oscar.Infrastructure.Features.Clients.Commands;
using Oscar.Infrastructure.Features.Clients.Queries;
using Oscar.Infrastructure.Contacts.Queries;
using Oscar.Core.Entities;

namespace Oscar.Blazor.Library.Components.Clients
{
    public partial class ClientForm
    {

        private ClientDto? _client;
        private IEnumerable<ContactDto> _existingContacts;

        private string ClientGradeString = "";
        readonly ClientValidator clientValidator = new();
        MudForm form;

        [Parameter]
        public int Id { get; set; } = 0;

        [Parameter]
        public bool Open { get; set; }

        [Parameter]
        public EventCallback<int> onSuccess { get; set; }

        [Parameter]
        public List<CountryDto>? Countries { get; set; }

        [Parameter]
        public IList<RightsTypeDto>? RightsType { get; set; }

        public EventCallback CloseClientDrawer { get; set; }
        protected string _buttonCaption => (_client != null && _client?.Id == 0) ? "Add Client" : "Update Client";
        protected string _formCaption => (_client != null && _client?.Id == 0) ? "Create New Client" : "Edit Client";


        protected override async Task OnParametersSetAsync()
        {
            await loadData();
        }

        protected async Task loadData(int clientId = 0)
        {
            if (clientId > 0) { Id = clientId; }
            _client = null;

            //var contactsResult = await Mediator.Send(new GetAllContactsQuery());
            //if (contactsResult.IsSuccess)
            //    _existingContacts = contactsResult.Value;

            if (Id > 0 && Open == true)
            {
                await base.OnParametersSetAsync();
                _client = (await Mediator.Send(new GetClientByIdQuery
                {
                    Id = Id
                })).Value;
                if (_client.Address == null)
                {
                    _client.Address = new();
                }
                StateHasChanged();
            }
            if (Id == 0 && Open == true)
            {
                await base.OnParametersSetAsync();
                _client = new()
                {
                    Address = new(),
                    Societies = new List<SocietyDto>(),
                    Documents = new List<DocumentDto>(),
                    Status = Status.Active_In_Term,
                    ClientGrade = ClientGrade.Silver,
                    ClientAltNames = new List<ClientAltNameDto>(),
                    Contacts = new List<ContactDto>(),
                    CustomerServiceManagers= new List<CustomerServiceManagerDto>(),
                };

                StateHasChanged();
            }

        }

        private async Task Submit()
        {
            await form.Validate();

            if (form.IsValid)
            {
                if (Id > 0)
                {
                    var updateClientCommand = new UpdateClientCommand
                    {
                        Id = _client.Id,
                        ClientUpdateDto = new ClientUpdateDto
                        {
                            ClientName = _client.ClientName,
                            Status = _client.Status,
                            Address = _client.Address != null && IsAddressAdded() ? new AddressAddDto
                            {
                                AddressLine1 = _client.Address.AddressLine1,
                                AddressLine2 = _client.Address.AddressLine2,
                                AddressLine3 = _client.Address.AddressLine3,
                                AddressLine4 = _client.Address.AddressLine4,
                                PostZipCode = _client.Address.PostZipCode,
                                WebSite = _client.Address.Website,
                                Country = _client.Address.Country,
                                IsCurrent = _client.Address.IsCurrent
                            } : null,
                            ClientGrade = _client.ClientGrade,
                            ClientType = _client.ClientType,
                            IMaestroClientCode = _client.IMaestroClientCode,
                            IMaestroGroupPayeeCode = _client.IMaestroGroupPayeeCode,
                            IMaestroGroupPayeeName = _client.IMaestroGroupPayeeName,
                            GeneralNotes = _client.GeneralNotes,
                            Contract = _client.Contract,
                            Societies = _client.Societies,
                            ClientAltNames = _client.ClientAltNames,
                            Contacts = _client.Contacts,
                            CustomServiceManagers = _client.CustomerServiceManagers,
                            AgicoaClientRef = _client.AgicoaClientRef,
                            CCCClientsId = _client.CCCClientsId,
                            CRCClientsId = _client.CRCClientsId,
                            MPAAClaimantsId = _client.MPAAClaimantsId,
                            ScreenRightsPortfolioId = _client.ScreenRightsPortfolioId,
                            ClientReference = _client.ClientReference
                        }
                    };

                    var result = await Mediator.Send(updateClientCommand);
                    await HandleResult(result, 0);
                }
                else
                {
                    var addClientCommand = new AddClientCommand
                    {
                        ClientAddDto = new ClientAddDto
                        {
                            ClientName = _client.ClientName,
                            Status = _client.Status,
                            Address = _client.Address != null && IsAddressAdded() ? new AddressAddDto
                            {
                                AddressLine1 = _client.Address.AddressLine1,
                                AddressLine2 = _client.Address.AddressLine2,
                                AddressLine3 = _client.Address.AddressLine3,
                                AddressLine4 = _client.Address.AddressLine4,
                                PostZipCode = _client.Address.PostZipCode,
                                WebSite = _client.Address.Website,
                                Country = _client.Address.Country,
                                IsCurrent = _client.Address.IsCurrent
                            } : null,
                            ClientGrade = _client.ClientGrade,
                            ClientType = _client.ClientType,
                            IMaestroClientCode = _client.IMaestroClientCode,
                            IMaestroGroupPayeeCode = _client.IMaestroGroupPayeeCode,
                            IMaestroGroupPayeeName = _client.IMaestroGroupPayeeName,
                            GeneralNotes = _client.GeneralNotes,
                            Contract = _client.Contract,
                            ClientAltNames = _client.ClientAltNames,
                            Contacts = _client.Contacts,
                            CustomServiceManagers = _client.CustomerServiceManagers,
                            AgicoaClientRef = _client.AgicoaClientRef,
                            CCCClientsId = _client.CCCClientsId,
                            CRCClientsId = _client.CRCClientsId,
                            MPAAClaimantsId = _client.MPAAClaimantsId,
                            ScreenRightsPortfolioId = _client.ScreenRightsPortfolioId
                        }
                    };

                    var result = await Mediator.Send(addClientCommand);
                    if (result.IsSuccess)
                    {
                        await loadData(result.Value.Id);
                        await HandleResult(result, result.Value.Id);
                    }
                    else
                    {
                        Snackbar.Add(result.Error, Severity.Error, config => { config.VisibleStateDuration = 9000; });
                    }
                }

            }
        }

        private async Task HandleResult(Result result, int newClientId)
        {
            if (result.IsSuccess)
            {
                Snackbar.Add($"Client successfully {(newClientId > 0 ? "created" : "updated")}", Severity.Success);
                if (newClientId == 0) { await form.ResetAsync(); }
                await SuccessCallback(-1) ;
            }
            else
            {
                Snackbar.Add(result.Error, Severity.Error, config => { config.VisibleStateDuration = 9000; });
            }
        }

        private async Task OnContactAdd()
        {
            //if (_client.Id == 0) return;

            ////_processing = true;

            //var result = await Mediator.Send(new UpdateClientCommand
            //{
            //    Id = _client.Id,
            //    ClientUpdateDto = new ClientUpdateDto()
            //    {

            //        ClientReference = _client.ClientReference,
            //        ClientName = _client.ClientName,
            //        Status = _client.Status,
            //        ClientGrade = _client.ClientGrade,
            //        ClientType = _client.ClientType,
            //        IMaestroClientCode = _client.IMaestroClientCode,
            //        Email = _client.Email,
            //        GeneralNotes = _client.GeneralNotes,
            //        Contract = _client.Contract,
            //        ClientAltNames = _client.ClientAltNames,
            //        Societies = _client.Societies,
            //        Contacts = _client.Contacts

            //    }
            //});
            ////_processing = false;

            //if (result.IsSuccess)
            //    Snackbar.Add("Client updated", Severity.Success);
            //else
            //    Snackbar.Add(result.Error, Severity.Error);
        }

        //TODO: Use validators from Infrastructure project - need to look at OscarContext pass/inject
        public class ClientValidator : AbstractValidator<ClientDto>
        {
            public ClientValidator()
            {

                RuleFor(r => r.ClientName)
                .NotNull()
                .NotEmpty();

                RuleFor(r => r.ClientGrade)
                    .NotNull()
                    .WithMessage("Client grade is required")
                    .IsInEnum()
                    .WithMessage("Client grade must be valid value");

                RuleFor(r => r.Status)
                       .NotNull()
                       .WithMessage("Client status is required")
                       .IsInEnum()
                       .WithMessage("Client status is required");

                When(r => r.Address != null && r.Address.Id > 0, () =>
                {
#pragma warning disable CS8602
                    RuleFor(r => r.Address.AddressLine1).NotEmpty().WithMessage("Address line 1 is required when including address");
                    RuleFor(r => r.Address.Country).NotEmpty().WithMessage("Country is required when including address");
                });

                When(r => r.Contract != null, () =>
                {
                    RuleFor(r => r.Contract.FirstStartDate).NotNull();
                    RuleFor(r => r.Contract.CurrentStartDate).NotNull();
                    RuleFor(r => r.Contract.EndDate).NotNull();
                    RuleFor(r => r.Contract.AutoRenew).NotNull();
                });
            }

            public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
            {
                var result = await ValidateAsync(ValidationContext<ClientDto>.CreateWithOptions((ClientDto)model, x => x.IncludeProperties(propertyName)));
                return result.IsValid ? Array.Empty<string>() : result.Errors.Select(e => e.ErrorMessage);
            };
        }


        public Boolean IsAddressAdded()
        {
            if (_client.Address is null) { return false; }
            if (_client.Address.Id == 0)
            {
                if (!String.IsNullOrEmpty(_client.Address.AddressLine1) ||
                    !String.IsNullOrEmpty(_client.Address.AddressLine2) ||
                    !String.IsNullOrEmpty(_client.Address.AddressLine3) ||
                    !String.IsNullOrEmpty(_client.Address.AddressLine4) ||
                    !String.IsNullOrEmpty(_client.Address.Country) ||
                    !String.IsNullOrEmpty(_client.Address.PostZipCode))
                {
                    return true;
                }

            }

            return false;
        }


        protected async Task SuccessCallback(int newClientId)
        {
            await onSuccess.InvokeAsync(newClientId);  
        }

        public IMask mask1 = new RegexMask(@"\p{Lu}");

        public PatternMask mask2 = new PatternMask("")
        {
            Transformation = AllUpperCase
        };

        // transform lower-case chars into upper-case chars
        private static char AllUpperCase(char c) => c.ToString().ToUpperInvariant()[0];

        protected async Task Cancel()
        {
            await onSuccess.InvokeAsync(-1);
        }
    }
}