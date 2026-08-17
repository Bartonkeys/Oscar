using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Oscar.Blazor.Library.Components;
using Oscar.Core.DTOs;
using Oscar.Infrastructure.Features.Registration.Queries;
using Oscar.Infrastructure.Features.Registration.Commands;
using static MudBlazor.CategoryTypes;

namespace Oscar.Blazor.Pages
{
    public partial class RegistrationSearch
    {
        private IEnumerable<RegistrationDisplayDto> registrations;
        private string _searchString = string.Empty;
        private bool _loading = true;

        private IEnumerable<RegistrationDisplayDto> pagedData;
        private MudTable<RegistrationDisplayDto> table;

        private int totalItems;
        private string searchByTitleString = null;


        private bool Filter(RegistrationDisplayDto registration) => FilterBySearchString(registration, _searchString);

        private static bool FilterBySearchString(RegistrationDisplayDto registrations, string searchString)
        {
            return string.IsNullOrWhiteSpace(searchString)
                   || string.IsNullOrWhiteSpace(registrations.SocietyName)
                   || registrations.SocietyName.Contains(searchString, StringComparison.OrdinalIgnoreCase);
        }


        private TableGroupDefinition<RegistrationDisplayDto> _groupDefinition = new()
        {

            GroupName = "Batch",
            Indentation = false,
            Expandable = true,
            IsInitiallyExpanded = false,
            Selector = (e) => e.RegistrationBatch.BatchId + " : "
                              + e.RegistrationBatch.DateRegistered + " : "
                              + e.RegistrationBatch.RegistrationCount + " registrations" + " : "
                              + "Status: " + e.RegistrationBatch.RegisterStatus
    };


        private async Task<TableData<RegistrationDisplayDto>> ServerReload(TableState state)
        {

            table.Loading = true;

            var getRegistrationsQuery = new GetRegistrationsQuery();
            if (searchByTitleString != null)
            {
                var SearchObjects = new List<SearchObject>();
                SearchObjects.Add(new SearchObject("Works.Titles", "string", "Title", searchByTitleString));
                getRegistrationsQuery.SearchObjects = SearchObjects;
            }
            if (state.SortLabel != null)
            {
                getRegistrationsQuery.SortColumn = state.SortLabel;
                getRegistrationsQuery.SortDirection = state.SortDirection == SortDirection.Descending ? "descending" : "ascending";
            }
            getRegistrationsQuery.Start = 0;
            getRegistrationsQuery.Take = int.MaxValue;

            var registrationsTable = (await Mediator.Send(getRegistrationsQuery)).Value;

            totalItems = registrationsTable.TotalRecords;
            pagedData = registrationsTable.Records.ToArray();
            _loading = false;
            return new TableData<RegistrationDisplayDto>() { TotalItems = totalItems, Items = pagedData };
        }

        private void OnSearchByTitle(string text)
        {
            _loading = true;
            searchByTitleString = text;
            table.ReloadServerData();
        }

        private async void DeleteRegistration(RegistrationDisplayDto registration)
        {
            var dialog = DialogService.Show<ConfirmDialog>("Delete Registration?");
            var dialogResult = await dialog.Result;

            if (!dialogResult.Cancelled)
            {
                var result = await Mediator.Send(new DeleteRegistrationCommand { Id = registration.Id });
                if (result.IsSuccess)
                    Snackbar.Add("Registration deleted", Severity.Success);
                else
                    Snackbar.Add(result.Error, Severity.Error, config => { config.VisibleStateDuration = 9000; });

                await table.ReloadServerData();
            }
        }
    }
}
