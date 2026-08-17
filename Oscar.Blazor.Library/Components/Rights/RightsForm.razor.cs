using BartonKeys.Functional;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Oscar.Core.Common;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Infrastructure.Features.Catalogue.Commands;
using Oscar.Infrastructure.Features.Clients.Queries;
using Oscar.Infrastructure.Features.Rights.Commands;
using Oscar.Infrastructure.Features.Rights.Queries;
using Severity = MudBlazor.Severity;

namespace Oscar.Blazor.Library.Components.Rights
{
    public partial class RightsForm
    {
        private bool _processing = false;
        readonly RightValidator rightValidator = new();
        private DateTime _defaultStartOfRight;
        private DateTime? _startOfRight;
        private DateTime? _endOfRight;
        private DateTime? _startOfValidity;
        private DateTime? _endOfValidity;
        private bool _endOfValidityPerpetuity;
        private bool _endOfRightPerpetuity;
        private IEnumerable<ChannelDto> _allChannels;
        private IEnumerable<LanguageDto> _allLanguages;
        private IEnumerable<CountryDto> _countries;
        private List<CountriesGroupsDto> _countriesGroups = new List<CountriesGroupsDto>();
        private RightDto _rightBeforeEdit;
        private bool _allRightsType { get; set; } = true;

        MudForm form;

        [Parameter]
        public RightsSource RightsSource { get; set; }

        [Parameter]
        public int ClientId { get; set; } = 0;

        [Parameter]
        public int WorksId { get; set; } = 0;

        [Parameter]
        public int? WorksProductionYear { get; set; }

        [Parameter]
        public int CatalogueId { get; set; } = 0;

        [Parameter]
        public bool Open { get; set; }

        [Parameter]
        public RightDto? Right { get; set; }

        [Parameter]
        public EventCallback<bool> RefreshParent { get; set; }

        [Parameter]
        public List<CountryDto>? Countries { get; set; }

        [Parameter]
        public IEnumerable<RightsTypeDto>? RightsType { get; set; }

        [Parameter]
        public EventCallback<bool> toggleDrawer { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            await LoadRightsType();
            await LoadAllChannels();
            await LoadAllLanguages();
            await LoadCountriesAndGroups();
            await CreateDefaultRight();

            if (Right != null)
            {
                _allRightsType = true;
                _rightBeforeEdit = Infrastructure.Features.Common.CloneHelper.Clone(Right);
                _startOfRight = Right.StartOfRight;
                _startOfValidity = Right.StartOfValidity;
                _endOfRight = Right.EndOfRight;
                _endOfValidity = Right.EndOfValidity;
                _endOfRightPerpetuity = Right.EndOfRight == Constants.Rights.Perpetuity;
                _endOfValidityPerpetuity = Right.EndOfValidity == Constants.Rights.Perpetuity;
            }
        }

        private void StartDateChanged(DateTime? date)
        {
            _startOfRight = date;
        }

        private void StartValidityChanged(DateTime? date)
        {
            _startOfValidity = date;
        }

        private void EndDateChanged(DateTime? date)
        {
            _endOfRight = date;
        }

        private void EndValidityChanged(DateTime? date)
        {
            _endOfValidity = date;
        }

        private void ToggleEndDatePerpetuity(bool toggled)
        {
            _endOfRightPerpetuity = toggled;
            _endOfRight = toggled ? Constants.Rights.Perpetuity : DateTime.Now;
        }

        private void ToggleEndValidityPerpetuity(bool toggled)
        {
            _endOfValidityPerpetuity = toggled;
            _endOfValidity = toggled ? Constants.Rights.Perpetuity : DateTime.Now;
        }

        private Variant GetVariant(bool isSelected)
        {
            return isSelected ? Variant.Filled : Variant.Outlined;
        }

        private async Task CreateDefaultRight()
        {
            if (Right != null) return;

            Right = new RightDto();
            _defaultStartOfRight = new DateTime(DateTime.Now.Year, 1, 1);
            if (WorksProductionYear != null && WorksProductionYear > 0)
            {
                _defaultStartOfRight = new DateTime(Convert.ToInt32(WorksProductionYear), 1, 1);
            }

            Right.StartOfRight = _defaultStartOfRight;
            Right.StartOfValidity = _defaultStartOfRight;
            Right.EndOfRight = Constants.Rights.Perpetuity;
            Right.EndOfValidity = Constants.Rights.Perpetuity;

            var rightsType = RightsType?.FirstOrDefault();
            if (rightsType != null)
            {
                Right.TypeId = rightsType.Id;
            }

            Right.ChannelRights = new List<ChannelRightsDto>();
            if (_allChannels.Any())
            {
                ChannelDto allChannels = _allChannels.FirstOrDefault(c => "*".Equals(c.Name, StringComparison.OrdinalIgnoreCase));
                if (allChannels != null)
                {
                    Right.ChannelRights.Add(new ChannelRightsDto { Channel = allChannels });
                }
            }

            Right.LanguageRights = new List<LanguageRightsDto>();
            if (_allLanguages.Any())
            {
                LanguageDto allLanguages = _allLanguages.FirstOrDefault(c => "*".Equals(c.Name, StringComparison.OrdinalIgnoreCase));
                if (allLanguages != null)
                {
                    Right.LanguageRights.Add(new LanguageRightsDto { Language = allLanguages });
                }
            }

            await LoadCountriesAndGroups();
            Right.Countries = new List<CountryDto>();
            Right.Percentage = 100;
            Right.CountriesGroups = new List<CountriesGroupsDto>(_countriesGroups);
            await Task.CompletedTask;
        }

        private async Task LoadRightsType()
        {
            if (RightsType != null) return;
            RightsType = await RefDataService.GetRightsType();
        }

        private async Task LoadAllChannels()
        {
            if (_allChannels != null) return;
            _allChannels = await RefDataService.GetAllChannels();
        }

        private async Task LoadAllLanguages()
        {
            if (_allLanguages != null) return;
            _allLanguages = await RefDataService.GetAllLanguages();
        }

        private async Task LoadCountries()
        {
            if (_countries != null) return;
            _countries = await RefDataService.GetCountries();
        }

        private async Task LoadCountriesAndGroups()
        {
            await LoadCountries();
            if (_countriesGroups == null) _countriesGroups = new List<CountriesGroupsDto>();

            if (_countriesGroups.Any()) { return; }

            var world = _countries.FirstOrDefault(x => x.Code == "*");
            if (world != null)
                _countriesGroups.Add(new CountriesGroupsDto { Id = world.Id, Code = world.Code, Name = world.Name });
            var canada = _countries.FirstOrDefault(x => x.Code == "CA");
            if (canada != null)
                _countriesGroups.Add(new CountriesGroupsDto { Id = canada.Id, Code = canada.Code, Name = canada.Name });
            var uk = _countries.FirstOrDefault(x => x.Code == "GB");
            if (uk != null)
                _countriesGroups.Add(new CountriesGroupsDto { Id = uk.Id, Code = uk.Code, Name = uk.Name });
            var usa = _countries.FirstOrDefault(x => x.Code == "US");
            if (usa != null)
                _countriesGroups.Add(new CountriesGroupsDto { Id = usa.Id, Code = usa.Code, Name = usa.Name });

            //Get all countries under their respective groups and then add them as well
            var allCountriesGroups = await RefDataService.GetCountryGroups();

            foreach (var group in allCountriesGroups)
            {
                if (group.HasChild)
                    _countriesGroups.Add(group);
            }

            _countriesGroups.AddRange(_countries.Where(x => x.Code != "*" && x.Code != "CA" && x.Code != "GB" && x.Code != "US")
                .Select(x => new CountriesGroupsDto { Id = x.Id, Code = x.Code, Name = x.Name }));
        }

        private async Task Submit()
        {
            _processing = true;
            await form.Validate();

            if (form.IsValid)
            {
                if (WorksProductionYear != null)
                {
                    if (_startOfRight < _defaultStartOfRight)
                    {
                        Snackbar.Add("Rights start date cannot be before production year", Severity.Error, config => { config.VisibleStateDuration = 9000; });
                        _processing = false;
                        return;
                    }
                    if (_startOfValidity < _defaultStartOfRight)
                    {
                        Snackbar.Add("Rights validity start date cannot be before production year", Severity.Error, config => { config.VisibleStateDuration = 9000; });
                        _processing = false;
                        return;
                    }
                }

                if (Right.Id > 0)
                {
                    var editRightCommand = new EditRightCommand()
                    {
                        RightAddDto = new RightAddDto
                        {
                            ID = Right.Id,
                            TypeID = Right.TypeId,
                            ClientID = ClientId,
                            Start = (DateTime)_startOfRight,
                            End = (DateTime)_endOfRight,
                            StartValidity = (DateTime)_startOfValidity,
                            EndValidity = (DateTime)_endOfValidity,
                            Notations = Right.Notations,
                            CatalogueID = await GetClientCatalgueId(),
                            Creation = DateTime.Now,
                            WorksID = WorksId,
                            Percentage = Right.Percentage,
                            ChannelIds = Right.ChannelRights.Select(a => a.Channel.Id).ToList(),
                            LanguageIds = Right.LanguageRights.Select(a => a.Language.Id).ToList(),
                            CountryIds = Right.Countries.Select(a => a.Id).ToList()
                        }
                    };

                    if (Right.TypeId == 99) //all rights
                    {
                        var clientRights = await GetSavedRights();

                        var editClientRights = clientRights.Value;
                        if (_allRightsType == true)
                        {
                            editClientRights = editClientRights.Where(
                                r => r.StartOfRight == _rightBeforeEdit.StartOfRight &&
                                     r.EndOfRight == _rightBeforeEdit.EndOfRight &&
                                     r.Percentage == _rightBeforeEdit.Percentage &&
                                     r.Countries != null && _rightBeforeEdit.Countries != null &&
                                     r.Countries.OrderBy(c => c.Id).SequenceEqual(_rightBeforeEdit.Countries.OrderBy(c => c.Id))
                            );
                        }

                        if (editClientRights != null)
                        {
                            foreach (var editClientRight in editClientRights)
                            {
                                editRightCommand.RightAddDto.ID = editClientRight.Id;
                                editRightCommand.RightAddDto.TypeID = editClientRight.TypeId;
                                var resultRight = await Mediator.Send(editRightCommand);
                            }
                            await HandleResult<RightDto>(Result.Ok(new RightDto()));
                        }
                    }
                    else
                    {
                        var resultRight = await Mediator.Send(editRightCommand);
                        await HandleResult<RightDto>(resultRight);
                    }
                }
                else
                {
                    var addRightCommand = new AddRightCommand()
                    {
                        RightAddDto = new RightAddDto
                        {
                            TypeID = Right.TypeId,
                            ClientID = ClientId,
                            Start = (DateTime)_startOfRight,
                            End = (DateTime)_endOfRight,
                            StartValidity = (DateTime)_startOfValidity,
                            EndValidity = (DateTime)_endOfValidity,
                            Notations = Right.Notations,
                            CatalogueID = await GetClientCatalgueId(),
                            Creation = DateTime.Now,
                            WorksID = WorksId,
                            Percentage = Right.Percentage,
                            ChannelIds = Right.ChannelRights.Select(a => a.Channel.Id).ToList(),
                            LanguageIds = Right.LanguageRights.Select(a => a.Language.Id).ToList(),
                            CountryIds = Right.Countries.Select(a => a.Id).ToList(),
                        },
                        RightsSource = RightsSource
                    };

                    if (Right.TypeId == 99) //all rights
                    {
                        foreach (var rightsTypeDto in RightsType.Where(r => r.Id != 99))
                        {
                            addRightCommand.RightAddDto.TypeID = rightsTypeDto.Id;
                            var resultRight = await Mediator.Send(addRightCommand);
                            await HandleResult<RightDto>(resultRight);
                        }
                    }
                    else
                    {
                        var resultRight = await Mediator.Send(addRightCommand);
                        await HandleResult<RightDto>(resultRight);
                    }

                }
            }
            _processing = false;
            await toggleDrawer.InvokeAsync(false);
        }

        private async Task<Result<IEnumerable<RightDto>>> GetSavedRights()
        {
            Result<IEnumerable<RightDto>> result;

            if (RightsSource == RightsSource.Works)
                result = await Mediator.Send(new GetRightsByWorksIdQuery { WorksId = WorksId });
            else
                result = (await Mediator.Send(new GetRightsByClientIdQuery()
                {
                    ClientId = ClientId
                }));

            return result;
        }
        private async Task<int> GetClientCatalgueId()
        {
            if (RightsSource == RightsSource.Client && CatalogueId == 0)
            {
                var clientResult = (await Mediator.Send(new GetClientByIdQuery
                {
                    Id = ClientId,
                }));

                var catalogue = clientResult.Value.Catalogues.FirstOrDefault(x => x.Name == clientResult.Value.ClientName);
                if (catalogue != null)
                    return catalogue.Id;

                var addCatalogueCommand = new AddCatalogueCommand()
                {
                    catalogueAddDto = new CatalogueAddDto()
                    {
                        Name = clientResult.Value.ClientName!,
                        ClientId = ClientId
                    }
                };
                var result = await Mediator.Send(addCatalogueCommand);
                return result.Value.Id;
            }
            return CatalogueId;
        }

        private async Task HandleResult<T>(Result<T> result)
        {
            if (result.IsSuccess)
            {
                Snackbar.Add("Right successfully saved", Severity.Success);
                await RefreshParent.InvokeAsync(true);
            }
            else
            {
                Snackbar.Add(result.Error, Severity.Error, config => { config.VisibleStateDuration = 9000; });
            }
        }
        public class RightValidator : AbstractValidator<RightDto>
        {
            public RightValidator()
            {
                RuleFor(x => x.TypeId).NotEmpty();
                RuleFor(x => x.StartOfRight).NotEmpty();
                RuleFor(x => x.EndOfRight).NotEmpty();
                RuleFor(x => x.StartOfRight)
                   .GreaterThan(x => x.EndOfRight)
                   .WithMessage("End date must be after the start date");
                RuleFor(x => x.StartOfValidity).NotEmpty();
                RuleFor(x => x.EndOfValidity).NotEmpty();
            }

            public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
            {
                var result = await ValidateAsync(ValidationContext<RightDto>.CreateWithOptions((RightDto)model, x => x.IncludeProperties(propertyName)));
                return result.IsValid ? Array.Empty<string>() : result.Errors.Select(e => e.ErrorMessage);
            };
        }

        protected async Task Cancel()
        {
            await toggleDrawer.InvokeAsync(false);
        }
    }
}
