using BartonKeys.Functional;
using Microsoft.AspNetCore.Components;
using Oscar.Core.DTOs;
using Severity = MudBlazor.Severity;
using FluentValidation;
using MudBlazor;
using Oscar.Infrastructure.Features.Clients.Queries;
using Oscar.Core.Enums;
using Oscar.Infrastructure.Features.Rights.Queries;
using Oscar.Infrastructure.Features.Rights.Commands;
using Oscar.Infrastructure.Features.Series.Queries;

namespace Oscar.Blazor.Library.Components.Rights
{
    public partial class RightsEdit
    {
        //private ClientDto? selectedClient;
        //private LightWeightWorksDto? selectedWork;
        private CatalogueDto? selectedCat;
        private MudTable<RightDto> table;
        //private string _searchString = null;
        private List<RightDto> _allRights;
        private List<RightDto> _rights;
        private IEnumerable<CatalogueDto?> _catalogues;

        //private IEnumerable<LightWeightWorksDto> _works = new List<LightWeightWorksDto>();
        private IEnumerable<RightsTypeDto> _rightsType = new List<RightsTypeDto>();

        //private MudAutocomplete<LightWeightWorksDto>? workSelect;
        private MudSelect<RightsTypeDto>? typeSelect;
        private MudDatePicker? startDate;
        private MudDatePicker? endDate;
        private MudDatePicker? startValid;
        private MudDatePicker? endValid;
        private MudTextField<string>? notations;
        private MudNumericField<decimal> percentage;

        private RightDto _selectedItem = new();
        private CatalogueDto? _selectedCatalogue;
        private ClientDto? _selectedClient;

        private DateTime _startRight;
        private DateTime _endRight;

        private DateTime _startValid;
        private DateTime _endValid;

        private decimal _percentage;

        private string _notations;

        [Parameter]
        public String CreateLabel { get; set; } = "";

        [Parameter]
        public bool Loading { get; set; }= true;

        [Parameter]
        public int Id { get; set; } = 0;

        [Parameter]
        public bool Works { get; set; } = false;

        [Parameter]
        public RightsSource RightsSource { get; set; }

        [Parameter]
        public bool Open { get; set; }

        [Parameter]
        public EventCallback onSuccess { get; set; }

        [Parameter]
        public int FilterClientID { get; set; }
        
        [Parameter]
        public int? FilterWorkID { get; set; }

        [Parameter]
        public int FilterCatID { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadRightsTypes();
        }

        protected override async Task OnParametersSetAsync()
        {
            if (Open & Id > 0) await LoadRights();
        }

        static string getRightsTypeString(RightsTypeDto input)
        {
            if (input == null)
                return string.Empty;

            string returnVal = input.Description;

            if (input.Name != null && input.Name != string.Empty)
                returnVal += " (" + input.Name + ")";

            return returnVal;
        }

        public Func<RightsTypeDto, string> converter = getRightsTypeString;

        //public async void onChange(EventArgs args)
        //{
        //    if (selectedClient != null)
        //        FilterClientID = selectedClient.Id;
        //    else
        //        FilterClientID = 0;

        //    //await table.ReloadServerData();
        //}

        //public async void onWorkChange(EventArgs args)
        //{
        //    if (selectedWork != null)
        //        FilterWorkID = selectedWork.Id;
        //    else
        //        FilterWorkID = 0;

        //    //await table.ReloadServerData();
        //}

        public async void onCatChange(EventArgs args)
        {
            if (selectedCat != null)
                FilterCatID = selectedCat.Id;
            else
                FilterCatID = 0;

            //await table.ReloadServerData();
        }

        //private async Task<IEnumerable<LightWeightWorksDto>> SearchWorks(string value)
        //{
        //    if (string.IsNullOrEmpty(value))
        //    {
        //        FilterWorkID = 0;
        //        selectedWork= null;
        //        return _works;
        //    }

        //    var filteredWorks = _works.Where(x => x.Title != null && x.Title.Contains(value, StringComparison.InvariantCultureIgnoreCase));

        //    return filteredWorks;
        //}

        //public void clientDropdownTextChanged()
        //{
        //    onChange(null);
        //}

        //public void workDropdownTextChanged()
        //{
        //    onWorkChange(null);
        //}

        //public void catDropdownTextChanged()
        //{
        //    onCatChange(null);
        //}

        private async Task ResetAddRightsFields()
        {
            //workSelect.Clear();
            await typeSelect.ClearAsync();
            await startDate.ClearAsync();
            await endDate.ClearAsync();
            await startValid.ClearAsync();
            await endValid.ClearAsync();
            await notations.ClearAsync();
            await percentage.ResetAsync();
        }

        private async void CreateRight()
        {
            var workID = FilterWorkID;
            var clientID = Id;

            if (Works)
            {
                workID = Id;
                clientID = _selectedClient.Id;
            }

            if (workID <= 0)
            {
                //Snackbar.Add("Please select a Work", Severity.Info);
                //return;
                workID = null;
            }

            if (typeSelect == null || typeSelect.Value == null)
            {
                Snackbar.Add("Please select a Right Type", Severity.Info);
                return;
            }
                

            if (startDate == null || startDate.Date == null)
            {
                Snackbar.Add("Please select a Start Date", Severity.Info);
                return;
            }

            if (endDate == null || endDate.Date == null)
            {
                Snackbar.Add("Please select an End Date", Severity.Info);
                return;
            }

            if (startDate.Date > endDate.Date)
            {
                Snackbar.Add("Please select a Start Date before the End Date", Severity.Info);
                return;
            }

            if (startValid == null || startValid.Date == null)
            {
                Snackbar.Add("Please Validity Start Date", Severity.Info);
                return;
            }

            DateTime endValidSelected = DateTime.MinValue;

            if (endValid != null && endValid.Date != null)
                endValidSelected = endValid.Date.Value;


            var addRightCommand = new AddRightCommand()
            {
                RightAddDto = new RightAddDto
                {
                    TypeID = typeSelect.Value.Id,
                    ClientID = clientID,
                    Start = startDate.Date.Value,
                    End = endDate.Date.Value,
                    StartValidity = startValid.Date.Value,
                    EndValidity = endValidSelected,
                    Notations = _notations,
                    CatalogueID = _selectedCatalogue.Id,
                    Creation = DateTime.Now,
                    WorksID = workID,
                    Percentage = _percentage
                }
            };
            var resultRight = await Mediator.Send(addRightCommand);
            await HandleResult<RightDto>(resultRight);

            await ResetAddRightsFields();
        }

        private async Task HandleResult<T>(Result<RightDto> result) where T : RightDto
        {
            if (result.IsSuccess)
            {
                _rights.Add(result.Value);
                StateHasChanged();
                Snackbar.Add("Successfully created Right", Severity.Success);
            }
            else
            {
                Snackbar.Add(result.Error, Severity.Error, config => { config.VisibleStateDuration = 9000; });
            }
        }

        private async Task LoadRights()
        {
            var result = Result.Fail<IEnumerable<RightDto>>("Not set");

            switch (RightsSource)
            {
                case RightsSource.Client:
                    result = await PopulateClientRights();
                    break;
                case RightsSource.Works:
                    result = await PopulateWorksRights();
                    break;
            }

            if (result.IsFailure) return;

            Loading = false;
        }

        private async Task LoadRightsTypes()
        {
            if (_rightsType == null || !_rightsType.Any())
                _rightsType = await RefDataService.GetRightsType();

            Loading = false;
        }

        //private string StyleForVisibility(bool visible)
        //{
        //    if (visible)
        //        return "";
            
        //    return "visibility: hidden";
        //}

        private async Task<Result<IEnumerable<RightDto>>> PopulateClientRights()
        {
            var clientResult = (await Mediator.Send(new GetClientByIdQuery
            {
                Id = Id,
            }));
            var result = (await Mediator.Send(new GetRightsByClientIdQuery()
            {
                ClientId = Id,
            }));

            _allRights = _rights = result.Value.ToList();
            _catalogues = result.Value.Where(c => c.Catalogue != null).Select(c => c.Catalogue).DistinctBy(c => c.Name);
            _selectedCatalogue = _catalogues != null && _catalogues.Any() ? _catalogues.SingleOrDefault(c => c.Name == clientResult.Value.ClientName) ?? _catalogues.First() : null;
            _rights = _allRights.Where(r => r.Catalogue?.Id == _selectedCatalogue?.Id).ToList();

            //LoadWorks();

            return result;
        }

        private async Task<Result<IEnumerable<RightsTypeDto>>> PopulateClientRightsType()
        {
            var result = (await Mediator.Send(new GetRightsTypeQuery() {}));

            return result;
        }

        //private async Task LoadWorks()
        //{
        //    if (Id > 0)
        //        _works = (await Mediator.Send(new GetWorksByClientQuery { ClientID = Id})).Value.ToList();
        //    else
        //        _works = (await Mediator.Send(new GetAllWorksQuery())).Value.ToList();
        //}

        private async Task<Result<IEnumerable<RightDto>>> PopulateWorksRights()
        {
            var result = (await Mediator.Send(new GetRightsByWorksIdQuery()
            {
                WorksId = Id,
            }));
            _catalogues = result.Value.Where(c => c.Catalogue != null).Select(c => c.Catalogue).DistinctBy(c => c.Name);
            _selectedCatalogue = _catalogues.FirstOrDefault();
            _allRights = _rights = result.Value.ToList();

            _selectedClient = (await Mediator.Send(new GetClientByIdQuery { Id = _selectedCatalogue.Client.Id })).Value;

            return result;
        }


        //private bool Filter(RightDto right) => FilterBySearchString(right, _searchString);

        //private static bool FilterBySearchString(RightDto right, string searchString)
        //{
        //    return string.IsNullOrWhiteSpace(searchString)
        //           || string.IsNullOrWhiteSpace(right.Type.Name)
        //           || right.Type.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)
        //           || (!string.IsNullOrWhiteSpace(right.Type.Description) && right.Type.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase))
        //           || right.StartOfRight.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase)
        //           || right.EndOfRight.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase)
        //           || right.StartOfValidity.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase)
        //           || right.EndOfValidity.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase);
        //}

        //private void LoadForClients(int id)
        //{
        //    _rights = _allRights.Where(r => r.Catalogue?.Id == id).ToList();
        //    _selectedCatalogue = _catalogues.SingleOrDefault(c => c?.Id == id);
        //}
    }
}