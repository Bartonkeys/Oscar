using BartonKeys.Functional;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Infrastructure.Features.Episode.Commands;
using Oscar.Infrastructure.Features.Episode.Queries;
using Oscar.Infrastructure.Features.MandateTypes.Queries;
using Severity = MudBlazor.Severity;

namespace Oscar.Blazor.Library.Components.Works
{
    public partial class EpisodeForm
    {

        private EpisodeDto? _works;
        private List<int> clientIds = new();
        private String OwningClient = "";
        private String OwningCatalogue = "";
        private List<CountryDto>? _countries;
        private List<PersonDto>? _actors;
        private List<PersonDto>? _directors;
        private List<PersonDto>? _producers;
        private bool _processing = false;
        private bool _showMandatesPanel = false;
        private bool IsReadOnlyClientAndCatPicker = false;
        private EventCallback showHideMandatesPanel => new(this, (Action<bool>)((bool isExpanded) => _showMandatesPanel = isExpanded));
        private ICollection<MandateTypeDto> _mandateTypes { get; set; } = new List<MandateTypeDto>();
        readonly WorksValidator worksValidator = new();
        MudForm form;

        [Parameter]
        public bool IsPage { get; set; }

        [Parameter]
        public int Id { get; set; } = 0;

        [Parameter]
        public int SeasonId { get; set; }

        [Parameter]
        public int SeriesId { get; set; }

        [Parameter]
        public WorksDto ParentWork { get; set; }

        [Parameter]
        public bool Open { get; set; }

        [Parameter]
        public EventCallback<bool> OpenChanged { get; set; }

        [Parameter]
        public EventCallback<bool> RefreshParent { get; set; }

        [Parameter]
        public Action<int, Discriminator, string> OpenParent { get; set; }

        [Parameter]
        public ICollection<EpisodeDto>? ExistingEpisodes { get; set; }

        [Parameter]
        public List<PersonDto> Actors { get; set; }

        [Parameter]
        public List<CountryDto>? Countries { get; set; }

        [Parameter]
        public IList<RightsTypeDto>? RightsType { get; set; }

        [Parameter]
        public EventCallback<bool> toggleWorksDrawer { get; set; }

        protected void ViewParent()
        {
            var title = _works.Titles.FirstOrDefault(t => t.TitleType == TitleType.Main || t.TitleType == TitleType.Episode)
                ?.Title;

            if (SeasonId > 0)
            {
                OpenParent(SeasonId, Discriminator.Season, title);
            }
            else if (SeriesId > 0)
            {
                OpenParent(SeriesId, Discriminator.Series, title);
            }
            else if (_works.Season != null)
            {
                OpenParent(_works.Season.Id, Discriminator.Season, title);
            }
            else if (_works.Series != null)
            {
                OpenParent(_works.Series.Id, Discriminator.Series,title);
            }
        }

        protected override async Task OnInitializedAsync()
        {

        }
        protected override async Task OnParametersSetAsync()
        {
            _works = null;

            await LoadMandateTypes();
            if (Id > 0 && Open == true)
            {
                await base.OnParametersSetAsync();
                _works = (await Mediator.Send(new GetEpisodeByIdQuery
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
                    _works.ClientReferences.Add(new ClientReferenceDto() { ClientId = _works.Clients.FirstOrDefault()?.Id ?? 0 });
                }

                IsReadOnlyClientAndCatPicker = true;
                StateHasChanged();
            }
            if (Id == 0 && Open == true)
            {
                await base.OnParametersSetAsync();
                _works = new()
                {
                    ProductionYear = ParentWork.ProductionYear,
                    FirstBroadcastYear = ParentWork.FirstBroadcastYear,
                    DurationMinutes = ParentWork.DurationMinutes,
                    WorksTypeId = ParentWork.WorksTypeId,
                    GenreId = ParentWork.GenreId,
                    WorksSubTypeId = ParentWork.WorksSubTypeId,
                    Countries = ParentWork.Countries,
                    Languages = ParentWork.Languages,
                    Directors = ParentWork.Directors,
                    Actors = ParentWork.Actors,
                    Producers = ParentWork.Producers,
                    Companies = ParentWork.Companies,
                    WorksStatus = Core.Enums.WorksStatus.Active,
                    CommissionedWorkStatus = Core.Enums.CommissionedWorkStatus.Unknown,
                    ReRegistrations = new List<ReRegistrationDto>()
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
                if (Id > 0)
                {
                    var updateEpisodeCommand = new UpdateEpisodeCommand
                    {
                        Id = _works.Id,
                        EpisodeUpdateDto = new EpisodeUpdateDto
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
                            AS400RefNo= _works.AS400RefNo,
                            CavcoCode = _works.CavcoCode,
                            CrtcCode = _works.CrtcCode,
                            GeneralNotes = _works.GeneralNotes,
                            Number = _works.Number,
                            Titles = _works.Titles,
                            SeasonId = _works.Season != null && _works.Season.Id != 0 ? _works.Season.Id : null,
                            SeriesId = _works.Series != null && _works.Series.Id != 0 ? _works.Series.Id : null,
                            ActorIds = _works.Actors.Select(a => a.Id).ToList(),
                            DirectorIds = _works.Directors.Select(a => a.Id).ToList(),
                            ScreenWriterIds = _works.ScreenWriters.Select(a => a.Id).ToList(),
                            ScriptWriterIds = _works.ScriptWriters.Select(a => a.Id).ToList(),
                            SocietyReferences = _works.SocietyReferences,
                            ClientReferences = _works.ClientReferences,
                            ProducerIds = _works?.Producers?.Select(a => a.Id).ToList(),
                            CompanyIds = _works.Companies.Select(a => a.Id).ToList(),
                            CountryIds = _works.Countries.Select(a => a.Id).ToList(),
                            LanguageIds = _works?.Languages?.Select(l => l.Id).ToList(),
                            ClientIds = clientIds,
                            CatalogueIds = _works.Catalogues?.Select(c => c.Id).ToList(),
                            ReRegistrations = _works.ReRegistrations,
                            MandateTypes = _mandateTypes
                        }
                    };
                    var result = await Mediator.Send(updateEpisodeCommand);
                    await HandleResult(result);
                }

                if (Id == 0)
                {
                    IDialogReference? dialog = null;
                    DialogResult? dialogResult = null;
                    if (IsDuplicate())
                    {
                        dialog = DialogService.Show<ConfirmDialog>("This episode already exists");
                        dialogResult = await dialog.Result;
                    }

                    if (dialog == null || !dialogResult.Cancelled)
                    {
                        var addEpisodeCommand = new AddEpisodeCommand
                        {
                            EpisodeAddDto = new EpisodeAddDto
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
                                SeasonId = SeasonId == 0 ? null : SeasonId,
                                SeriesId = SeriesId == 0 ? null : SeriesId,
                                CountryIds = _works.Countries.Select(a => a.Id).ToList(),
                                DirectorIds = _works.Directors.Select(d => d.Id).ToList(),
                                LanguageIds = _works?.Languages?.Select(l => l.Id).ToList(),
                                ActorIds = _works?.Actors?.Select(a => a.Id).ToList(),
                                ProducerIds = _works?.Producers?.Select(a => a.Id).ToList(),
                                CompanyIds = _works?.Companies?.Select(c => c.Id).ToList(),
                                MandateTypes = _mandateTypes
                            }
                        };

                        var result = await Mediator.Send(addEpisodeCommand);
                        await HandleResult(result);
                    }
                }
            }
            _processing = false;
        }

        private async Task HandleResult<T>(Result<T> result)
        {
            if (result.IsSuccess)
            {
                Snackbar.Add("Episode successfully saved", Severity.Success);
                Open = false;
                await OpenChanged.InvokeAsync(Open);
                //await RefreshParent.InvokeAsync(true);
            }
            else
            {
                Snackbar.Add(result.Error, Severity.Error, config => { config.VisibleStateDuration = 9000; });
            }
        }

        private bool IsDuplicate()
        {
            if (ExistingEpisodes != null && ExistingEpisodes.Any())
            {
                return ExistingEpisodes.Any(s =>
                    s.Titles.Any(t => t.TitleType == TitleType.Main && t.Title == _works.Titles.First(newTitle => newTitle.TitleType == TitleType.Main).Title) &&
                    s.ProductionYear == _works.ProductionYear &&
                    s.GenreId == _works.GenreId &&
                    s.DurationMinutes == _works.DurationMinutes &&
                    s.Countries.Any(c => _works.Countries.Any(cc => cc.Id == c.Id))
                );
            }
            return false;
        }

        public class WorksValidator : AbstractValidator<EpisodeDto>
        {
            public WorksValidator()
            {
                RuleFor(x => x.WorksStatus)
                   .NotNull();

                When(x => x.WorksStatus != Core.Enums.WorksStatus.Uncontrolled, () => {
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
                var result = await ValidateAsync(ValidationContext<EpisodeDto>.CreateWithOptions((EpisodeDto)model, x => x.IncludeProperties(propertyName)));
                return result.IsValid ? Array.Empty<string>() : result.Errors.Select(e => e.ErrorMessage);
            };
        }

        protected async Task Cancel()
        {
            await toggleWorksDrawer.InvokeAsync(false);
        }

        private async Task StatusChanged(WorksStatus? workStatus)
        {
            _works.WorksStatus = workStatus;
        }

        private async Task ConflictsChanged(ICollection<ConflictDto> conflictDtos)
        {
            _works.Conflicts = conflictDtos;
        }

    }
}
