using BartonKeys.Functional;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Infrastructure.Features.MandateTypes.Queries;
using Oscar.Infrastructure.Features.Rights.Queries;
using Oscar.Infrastructure.Features.StandAlone.Commands;
using Oscar.Infrastructure.Features.StandAlone.Queries;
using System.Drawing.Printing;
using System.Linq;
using Severity = MudBlazor.Severity;

namespace Oscar.Blazor.Library.Components.Works
{
    public partial class StandaloneForm
    {
        private StandAloneDto? _works;
        private List<CountryDto> _countries;
        //private List<PersonDto>? _actors;
        private List<PersonDto>? _producers;
        private List<PersonDto>? _distributors;
        private bool _showMandatesPanel = false;
        private bool IsReadOnlyClientAndCatPicker = false;
        private EventCallback showHideMandatesPanel => new(this, (Action<bool>)((bool isExpanded) => _showMandatesPanel = isExpanded));
        private ICollection<MandateTypeDto> _mandateTypes { get; set; } = new List<MandateTypeDto>();

        private bool _processing = false;
        readonly WorksValidator worksValidator = new();
        MudForm form;
        private List<int> clientIds = new();

        [Parameter]
        public int Id { get; set; } = 0;

        //[Parameter]
        //public List<PersonDto> Actors { get; set; }

        [Parameter]
        public List<CountryDto>? Countries { get; set; }

        [Parameter]
        public IList<RightsTypeDto>? RightsType { get; set; }

        [Parameter]
        public bool Open { get; set; }

        [Parameter]
        public bool IsPage { get; set; }

        [Parameter]
        public EventCallback<bool> toggleWorksDrawer { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            await loadData();
        }

        protected override async Task OnInitializedAsync()
        {
            //_countries = (await Mediator.Send(new GetAllCountriesQuery())).Value.OrderBy(x => x.Name).ToList();
            //_actors = await LoadPersonsAsync(PersonType.Actor);
            //_producers = await LoadPersonsAsync(PersonType.Producer);
            //_distributors = await LoadPersonsAsync(PersonType.Distributor);
            _showMandatesPanel = false;
        }

        protected async Task loadData()
        {
            _works = null;
            await LoadMandateTypes();

            if (Id > 0 && Open == true)
            {
                await base.OnParametersSetAsync();

                _works = (await Mediator.Send(new GetStandAloneByIdQuery
                {
                    Id = Id
                })).Value;
                clientIds = _works.Clients.Select(a => a.Id).ToList();

                if ((_works?.ClientReferences != null) && (_works?.ClientReferences.Count() == 0))
                {
                    _works.ClientReferences.Add(new ClientReferenceDto() { ClientId = _works.Clients.First().Id });
                }
                IsReadOnlyClientAndCatPicker = true;
                StateHasChanged();
            }
            if (Id == 0 && Open == true)
            {
                await base.OnParametersSetAsync();
                _works = new()
                {
                    WorksStatus = WorksStatus.Active,
                    CommissionedWorkStatus = CommissionedWorkStatus.Unknown,
                    Countries = new List<CountryDto>(),
                    Directors = new List<PersonDto>(),
                    Companies = new List<CompanyDto>(),
                    Languages = new List<LanguageDto>(),
                    ReRegistrations = new List<ReRegistrationDto>(),
                    Clients = new List<ClientDto>(),
                    Catalogues = new List<CatalogueDto>()
                };
                IsReadOnlyClientAndCatPicker = false;
                StateHasChanged();
            }
        }

        private async Task LoadMandateTypes()
        {
            _mandateTypes = (await Mediator.Send(new GetAllMandateTypesQuery())).Value.ToList();
            var mandates = _works?.Mandates.Where(x => x.Mandated).ToList();

            if (mandates != null && mandates.Any())
            {
                foreach (var mandate in mandates)
                {
                    var m = _mandateTypes.FirstOrDefault(x => x.Id == mandate.MandateType.Id);
                    if (m != null) { m.Mandated = true; }
                }
            }
            else //default to true when no mandates already exists
            {
                foreach (var mandateType in _mandateTypes)
                    mandateType.Mandated = true;
            }
        }

        private async Task Submit()
        {
            _processing = true;
            await form.Validate();

            if (form.IsValid)
            {
                if (!ValidateClientCatalogue())
                {
                    _processing = false;
                    return;
                }

                if (Id > 0)
                {
                    var updateStandAloneCommand = new UpdateStandAloneCommand
                    {
                        Id = _works.Id,
                        StandAloneUpdateDto = new StandAloneUpdateDto
                        {
                            WorksStatus = _works.WorksStatus,
                            UncontrolledReason = _works.UncontrolledReason,
                            CommissionedWorkStatus = _works.CommissionedWorkStatus,
                            GenreId = _works.GenreId,
                            WorksSubTypeId = _works.WorksSubTypeId,
                            WorksTypeId = _works.WorksTypeId,
                            DurationMinutes = _works.DurationMinutes,
                            ProductionYear = _works.ProductionYear,
                            FirstBroadcastYear = _works.FirstBroadcastYear,
                            IMaestroWorkCode = _works.IMaestroWorkCode,
                            Isan = _works.Isan,
                            AS400RefNo = _works.AS400RefNo,
                            CavcoCode = _works.CavcoCode,
                            CrtcCode = _works.CrtcCode,
                            GeneralNotes = _works.GeneralNotes,
                            Titles = _works.Titles,
                            ActorIds = _works.Actors.Select(a => a.Id).ToList(),
                            DirectorIds = _works.Directors.Select(a => a.Id).ToList(),
                            ProducerIds = _works.Producers.Select(a => a.Id).ToList(),
                            DistributorIds = _works.Distributors.Select(a => a.Id).ToList(),
                            ScreenWriterIds = _works.ScreenWriters.Select(a => a.Id).ToList(),
                            ScriptWriterIds = _works.ScriptWriters.Select(a => a.Id).ToList(),
                            CompanyIds = _works.Companies.Select(a => a.Id).ToList(),
                            CountryIds = _works.Countries.Select(a => a.Id).ToList(),
                            CatalogueIds = _works.Catalogues.Select(a => a.Id).ToList(),
                            ClientIds = clientIds,
                            SocietyReferences = _works.SocietyReferences,
                            ClientReferences = _works.ClientReferences,
                            LanguageIds = _works.Languages.Select(l => l.Id).ToList(),
                            ReRegistrations = _works.ReRegistrations,
                            MandateTypes = _mandateTypes,
                            CompactRef = _works.CompactRef,
                            AgicoaWorksReference = _works.AgicoaWorksReference
                        }
                    };

                    var result = await Mediator.Send(updateStandAloneCommand);
                    await HandleResult(result);
                }
                if (Id == 0)
                {
                    bool IsDuplicate = (await Mediator.Send(new StandAloneSearchForDuplicate
                    {
                        Title = _works.Titles.First(t => t.TitleType == Core.Enums.TitleType.Main).Title,
                        CountryIds = _works.Countries.Select(c => c.Id).ToList(),
                        DurationMinutes = _works.DurationMinutes,
                        GenreId = _works.GenreId,
                        ProductionYear = _works.ProductionYear
                    })).Value;

                    IDialogReference? dialog = null;
                    DialogResult? dialogResult = null;
                    if (IsDuplicate)
                    {
                        dialog = DialogService.Show<ConfirmDialog>("This standalone already exists");
                        dialogResult = await dialog.Result;
                    }

                    if (dialog == null || !dialogResult.Cancelled)
                    {
                        var addStandAloneCommand = new AddStandAloneCommand
                        {
                            StandAloneAddDto = new StandAloneAddDto
                            {
                                WorksStatus = _works.WorksStatus,
                                UncontrolledReason = _works.UncontrolledReason,
                                CommissionedWorkStatus = _works.CommissionedWorkStatus,
                                GenreId = _works.GenreId,
                                GenreSubTypeId = _works.GenreSubTypeId,
                                WorksTypeId = _works.WorksTypeId,
                                DurationMinutes = _works.DurationMinutes,
                                ProductionYear = _works.ProductionYear,
                                FirstBroadcastYear = _works.FirstBroadcastYear,
                                IMaestroWorkCode = _works.IMaestroWorkCode,
                                AgicoaWorksReference = _works.AgicoaWorksReference,
                                Isan = _works.Isan,
                                AS400RefNo = _works.AS400RefNo,
                                CavcoCode = _works.CavcoCode,
                                CrtcCode = _works.CrtcCode,
                                GeneralNotes = _works.GeneralNotes,
                                Titles = _works.Titles,
                                CountryIds = _works.Countries.Select(a => a.Id).ToList(),
                                DirectorIds = _works.Directors.Select(d => d.Id).ToList(),
                                CompanyIds = _works.Companies.Select(c => c.Id).ToList(),
                                LanguageIds = _works.Languages.Select(l => l.Id).ToList(),
                                ClientIds = clientIds,
                                CatalogueIds = _works.Catalogues.Select(a => a.Id).ToList(),
                                MandateTypes = _mandateTypes
                            }
                        };

                        var result = await Mediator.Send(addStandAloneCommand);
                        if (result.IsSuccess)
                        {
                            Id = result.Value.Id;
                            await loadData();
                            Snackbar.Add("Standalone successfully saved", Severity.Success);
                        }
                        else
                        {
                            Snackbar.Add(result.Error, Severity.Error, config => { config.VisibleStateDuration = 9000; });
                        }
                    }
                }
            }
            _processing = false;
        }

        private bool ValidateClientCatalogue()
        {
            if ((clientIds == null) || (clientIds.Count() == 0))
            {
                Snackbar.Add("Client is required.", Severity.Error);
                return false;
            }
            if ((_works.Catalogues == null) || (_works.Catalogues.Count() == 0))
            {
                Snackbar.Add("Catalogue is required.", Severity.Error);
                return false;
            }

            return true;
        }

        private async Task HandleResult<T>(Result<T> result)
        {
            if (result.IsSuccess)
            {
                Snackbar.Add("Standalone successfully saved", Severity.Success);
            }
            else
            {
                Snackbar.Add(result.Error, Severity.Error, config => { config.VisibleStateDuration = 9000; });
            }
        }
        public class WorksValidator : AbstractValidator<StandAloneDto>
        {
            public WorksValidator()
            {
                RuleFor(x => x.WorksStatus)
                   .NotNull();

                When(x => x.WorksStatus != WorksStatus.Uncontrolled, () => {
                    RuleFor(x => x.ProductionYear)
                    .NotEmpty();
                    RuleFor(x => x.DurationMinutes)
                   .NotEmpty()
                   .GreaterThan(0);
                    RuleFor(x => x.GenreId)
                  .NotNull()
                  .GreaterThan(0).WithMessage("Genre is required");
                });

                When(x => x.WorksStatus == WorksStatus.Uncontrolled, () =>
                {
                    RuleFor(r => r.UncontrolledReason).NotEmpty().WithMessage($"Uncontrolled Work items must have a reason specified");
                });
            }

            public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
            {
                var result = await ValidateAsync(ValidationContext<StandAloneDto>.CreateWithOptions((StandAloneDto)model, x => x.IncludeProperties(propertyName)));
                return result.IsValid ? Array.Empty<string>() : result.Errors.Select(e => e.ErrorMessage);
            };
        }

        protected async Task Cancel()
        {
            await toggleWorksDrawer.InvokeAsync(false);
        }

        private async Task WorkStatusChanged(WorksStatus? workStatus)
        {
            _works.WorksStatus = workStatus;
        }

        private async Task ConflictsChanged(ICollection<ConflictDto> conflictDtos)
        {
            _works.Conflicts = conflictDtos;
        }

    }
}
