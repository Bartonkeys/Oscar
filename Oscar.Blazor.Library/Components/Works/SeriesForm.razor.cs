using BartonKeys.Functional;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Infrastructure.Features.MandateTypes.Queries;
using Oscar.Infrastructure.Features.Series.Commands;
using Oscar.Infrastructure.Features.Series.Queries;
using static Oscar.Blazor.Library.Components.CopyForm;
using static Oscar.Blazor.Library.Components.Works.SeasonsEpisodesTree;
using Severity = MudBlazor.Severity;

namespace Oscar.Blazor.Library.Components.Works
{
    public partial class SeriesForm
    {
        private SeriesDto? _works;
        private bool _processing = false;
        readonly WorksValidator worksValidator = new();
        MudForm form;
        private List<int> clientIds = new();
        private String OwningClient = "";
        private String OwningCatalogue = "";
        private List<CountryDto> _countries;
        private List<PersonDto>? _actors;
        private List<PersonDto>? _producers;
        private List<PersonDto>? _distributors;
        private List<PersonDto>? _directors;
        private HashSet<WorksDto> selectedItems = new();
        private bool openCopyDrawer;
        private string actionText = "";
        private bool _showMandatesPanel = false;
        private bool IsReadOnlyClientAndCatPicker = false;
        private EventCallback showHideMandatesPanel => new(this, (Action<bool>)((bool isExpanded) => _showMandatesPanel = isExpanded));
        private ICollection<MandateTypeDto> _mandateTypes { get; set; } = new List<MandateTypeDto>();

        [Parameter]
        public bool IsPage { get; set; }
        [Parameter]
        public int Id { get; set; } = 0;

        [Parameter]
        public bool Open { get; set; }

        [Parameter]
        public EventCallback<bool> OpenChanged { get; set; }

        [Parameter]
        public Action<int, Discriminator, string> OpenParent { get; set; }

        //[Parameter]
        //public List<PersonDto> Actors { get; set; }

        [Parameter]
        public List<CountryDto>? Countries { get; set; }

        [Parameter]
        public IList<RightsTypeDto>? RightsType { get; set; }

        [Parameter]
        public EventCallback<bool> toggleWorksDrawer { get; set; }

        public bool UpdateAllStatus { get; set; } = false;
        public bool ShowUpdateAllStatus { get; private set; }

        protected async void refresh()
        {
            await loadData();
        }

        protected override async Task OnParametersSetAsync()
        {
            await loadData();
        }

        protected override async Task OnInitializedAsync()
        {


        }

        protected async Task loadData()
        {
            _works = null;

            await LoadMandateTypes();
            if (Id > 0 && Open == true)
            {
                await base.OnParametersSetAsync();
                _works = (await Mediator.Send(new GetSeriesByIdQuery
                {
                    Id = Id
                })).Value;
                clientIds = _works.Clients.Select(a => a.Id).ToList();
                if (_works.Clients != null && _works.Clients.Count() > 0)
                {
                    OwningClient = _works.Clients.First().ClientName;
                }
                if (_works.Catalogues != null && _works.Catalogues.Count() > 0)
                {
                    OwningCatalogue = _works.Catalogues.First().Name;
                }

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
                    WorksStatus = Core.Enums.WorksStatus.Active,
                    CommissionedWorkStatus = Core.Enums.CommissionedWorkStatus.Unknown,
                    Countries = new List<CountryDto>(),
                    Directors = new List<PersonDto>(),
                    Languages = new List<LanguageDto>(),
                    ReRegistrations = new List<ReRegistrationDto>(),
                    Companies = new List<CompanyDto>(),
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
                    var updateSeriesCommand = new UpdateSeriesCommand
                    {
                        Id = _works.Id,
                        SeriesUpdateDto = new SeriesUpdateDto
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
                            CompactRef = _works.CompactRef,
                            AgicoaWorksReference = _works.AgicoaWorksReference,
                            Isan = _works.Isan,
                            AS400RefNo = _works.AS400RefNo,
                            CavcoCode = _works.CavcoCode,
                            CrtcCode = _works.CrtcCode,
                            GeneralNotes = _works.GeneralNotes,
                            Number = _works.Number,
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
                            LanguageIds = _works?.Languages?.Select(l => l.Id).ToList(),
                            ReRegistrations = _works.ReRegistrations,
                            MandateTypes = _mandateTypes,
                            UpdateAllStatus = UpdateAllStatus
                        }
                    };

                    var result = await Mediator.Send(updateSeriesCommand);
                    await HandleResult(result);
                }
                if (Id == 0)
                {
                    List<DuplicateDto> duplicates = (await Mediator.Send(new SeriesSearchForDuplicate
                    {
                        Title = _works.Titles.First(t => t.TitleType == Core.Enums.TitleType.Main).Title,
                        CountryIds = _works.Countries.Select(c => c.Id).ToList(),
                        DurationMinutes = _works.DurationMinutes,
                        GenreId = _works.GenreId,
                        ProductionYear = _works.ProductionYear
                    })).Value;

                    IDialogReference? dialog = null;
                    DialogResult? dialogResult = null;
                    if (duplicates.Count() > 0)
                    {
                        var parameters = new DialogParameters { ["duplicates"] = duplicates };
                        dialog = DialogService.Show<DuplicateDialog>("Dup detected", parameters);
                        dialogResult = await dialog.Result;
                    }

                    if (dialog == null || !dialogResult.Cancelled)
                    {
                        var addSeriesCommand = new AddSeriesCommand
                        {
                            SeriesAddDto = new SeriesAddDto
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
                                AgicoaWorksReference = _works.AgicoaWorksReference,
                                Isan = _works.Isan,
                                AS400RefNo = _works.AS400RefNo,
                                CavcoCode = _works.CavcoCode,
                                CrtcCode = _works.CrtcCode,
                                GeneralNotes = _works.GeneralNotes,
                                Number = _works.Number,
                                Titles = _works.Titles,
                                CountryIds = _works.Countries.Select(a => a.Id).ToList(),
                                DirectorIds = _works.Directors.Select(d => d.Id).ToList(),
                                LanguageIds = _works?.Languages?.Select(d => d.Id).ToList(),
                                CompanyIds = _works?.Companies?.Select(c => c.Id).ToList(),
                                ClientIds = clientIds,
                                CatalogueIds = _works.Catalogues.Select(a => a.Id).ToList(),
                                MandateTypes = _mandateTypes
                            }
                        };

                        var result = await Mediator.Send(addSeriesCommand);
                        if (result.IsSuccess)
                        {
                            Id = result.Value.Id;
                            loadData();
                            Snackbar.Add("Series successfully saved", Severity.Success);
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
                await loadData();
                Snackbar.Add("Series successfully saved", Severity.Success);
            }
            else
            {
                Snackbar.Add(result.Error, Severity.Error, config => { config.VisibleStateDuration = 9000; });
            }
        }

        public class WorksValidator : AbstractValidator<SeriesDto>
        {
            public WorksValidator()
            {
                RuleFor(x => x.WorksStatus)
                   .NotNull();

                When(x => x.WorksStatus != Core.Enums.WorksStatus.Uncontrolled, () =>
                {
                    RuleFor(x => x.ProductionYear)
                    .NotEmpty();
                    RuleFor(x => x.DurationMinutes)
                   .NotEmpty()
                   .GreaterThan(0);
                    RuleFor(x => x.GenreId)
                  .NotNull()
                  .GreaterThan(0).WithMessage("Genre is required");
                });

                When(x => x.WorksStatus == Core.Enums.WorksStatus.Uncontrolled, () =>
                {
                    RuleFor(r => r.UncontrolledReason).NotEmpty().WithMessage($"Uncontrolled Work items must have a reason specified");
                });
            }

            public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
            {
                var result = await ValidateAsync(ValidationContext<SeriesDto>.CreateWithOptions((SeriesDto)model, x => x.IncludeProperties(propertyName)));
                
                Console.WriteLine("propertyName: " + propertyName);
                Console.WriteLine("result.IsValid: " + result.IsValid);
                return Array.Empty<string>();
                //return result.IsValid ? Array.Empty<string>() : result.Errors.Select(e => e.ErrorMessage);
            };
        }

        protected async Task Cancel()
        {
            await toggleWorksDrawer.InvokeAsync(false);
        }

        private void OpenCopyDrawer(SeriesDto series)
        {
            openCopyDrawer = true;
            selectedItems.Clear();
            selectedItems.Add(new WorksDto
            {
                Id = series.Id,
                Discriminator = series.Discriminator.ToString(),
                ProductionYear = series.ProductionYear,
                Titles = new List<WorksTitleDto>(){
                            new WorksTitleDto() {
                                Title = series.Titles.FirstOrDefault().Title
                            }
                        }
            });
        }

        private async Task CopyFormComplete(CopyFormResponse response)
        {
            openCopyDrawer = false;

            if (response.Response)
            {
                var actionString = "Copied";

                if (response.Relinquish)
                    actionString = "Moved";

                actionText = selectedItems.Count() + " Works " + actionString + " to " + response.Client.ClientName;

                if (response.Catalogue != null)
                    actionText += "/" + response.Catalogue.Name;

                if (response.Response)
                    Snackbar.Add(actionText);

                selectedItems = new HashSet<WorksDto>();
                StateHasChanged();
            }
        }

        private async Task WorkStatusChanged(WorksStatus? workStatus)
        {
            ShowUpdateAllStatus = true;
            _works.WorksStatus = workStatus;
        }

        private async Task ConflictsChanged(ICollection<ConflictDto> conflictDtos)
        {
            _works.Conflicts = conflictDtos;
        }
    }
}
