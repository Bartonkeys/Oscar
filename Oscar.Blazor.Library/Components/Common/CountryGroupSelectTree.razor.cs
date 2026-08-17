using LinqKit;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Oscar.Core.DTOs;
using Oscar.Infrastructure.Features.Common;

namespace Oscar.Blazor.Library.Components.Common
{
    public partial class CountryGroupSelectTree
    {
        [Parameter]
        public HashSet<CountriesGroupsDto>? CountriesGroups { get; set; }

        [Parameter]
        public ICollection<CountryDto>? Countries { get; set; }

        [Parameter]
        public EventCallback<HashSet<CountriesGroupsDto>> SelectedValuesChanged { get; set; }

        [Parameter]
        public String Header { get; set; } = "";

        private MudTreeView<CountriesGroupsDto>? countriesGroupsTreeView;
        private MudCheckBox<bool>? showSelected;
        private HashSet<CountriesGroupsDto>? CountriesGroupsCopy { get; set; }
        private string searchValue = string.Empty;


        protected override async Task OnParametersSetAsync()
        {
            CountriesGroupsCopy = CloneHelper.Clone(CountriesGroups);
            SetDefaultChecked();
            Refresh();
            if (showSelected != null)
            {
                ShowSelected(showSelected.Checked);
            }

            await Task.CompletedTask;
        }

        private void SetDefaultChecked()
        {
            if (Countries?.Count() == 0)
            {
                var world = CountriesGroups?.FirstOrDefault(x => x.Code == "*");
                if (world != null)
                {
                    world.IsChecked = true;
                    AddRemoveCountry(world);
                }
            }
        }

        private void Refresh()
        {
            SetIsCheckedFalse();
            SetSelectedCountriesChecked();
        }

        private void ClearAll()
        {
            SetIsCheckedFalse();
            Countries?.Clear();
        }

        private void SetIsCheckedFalse()
        {
            //ResetUnChecked(countriesGroupsTreeView?.Items);
            ResetUnChecked(CountriesGroups);
            ResetUnChecked(CountriesGroupsCopy);
        }

        private void ResetUnChecked(HashSet<CountriesGroupsDto> countriesGroups)
        {
            if (countriesGroups != null && countriesGroups.Any())
            {
                foreach (var item in countriesGroups)
                {
                    item.IsChecked = false;
                    if (item.HasChild)
                    {
                        foreach (var child in item.Children)
                            child.IsChecked = false;
                    }
                }
            }
        }

        private void SetSelectedCountriesChecked()
        {
            if (Countries != null)
            {
                foreach (var country in Countries)
                {
                    ResetChecked(country, CountriesGroups);
                    ResetChecked(country, CountriesGroupsCopy);
                }
            }
        }

        private void ResetChecked(CountryDto country, HashSet<CountriesGroupsDto> countriesGroups)
        {
            if (countriesGroups != null && countriesGroups.Any())
            {
                foreach (var countriesGroup in countriesGroups)
                {
                    //This could match with a CountryGroup hence make sure its a Country by matching with code as well
                    if (countriesGroup.Id == country.Id && countriesGroup.Code == country.Code)
                        countriesGroup.IsChecked = true;

                    SetSelectedChildCountriesChecked(countriesGroup, country);
                }
            }
        }

        private void SetSelectedChildCountriesChecked(CountriesGroupsDto countriesGroup, CountryDto country)
        {
            if (countriesGroup.HasChild)
            {
                foreach (CountriesGroupsDto child in countriesGroup.Children)
                {
                    if (child.Id == country.Id && child.Code == country.Code)
                        child.IsChecked = true;
                }
                countriesGroup.IsChecked = countriesGroup.Children.All(i => i.IsChecked);
            }
        }

        protected void ShowSelected(bool isChecked)
        {
            var clonedItems = CloneHelper.Clone(CountriesGroupsCopy);

            if (clonedItems == null) return;

            if (isChecked)
            {
                clonedItems = clonedItems.Where(x => x.IsChecked).ToHashSet();
            }

            CountriesGroups = clonedItems;
            Refresh();
        }

        private void Search()
        {
            var clonedItems = CloneHelper.Clone(CountriesGroupsCopy);
            if (clonedItems == null || !clonedItems.Any()) return;

            CountriesGroups = clonedItems
                .Where(x => x.Name
                .StartsWith(searchValue, StringComparison.InvariantCultureIgnoreCase))
                .ToHashSet();
        }


        protected void TreeItemCheckedChanged(CountriesGroupsDto item)
        {
            item.IsChecked = !item.IsChecked;

            //If item don't have any child that means its a Country and not a CountryGroup
            //AddRemove country but not CountryGroup for saving updated countries collection
            if (!item.HasChild)
                AddRemoveCountry(item);

            // checked status on any child items should mirrror this parent item
            if (item.HasChild)
            {
                foreach (CountriesGroupsDto child in item.Children)
                {
                    child.IsChecked = item.IsChecked;
                    AddRemoveCountry(child);
                }
            }
            // if there's a parent and all children are checked/unchecked, parent should match
            if (item.Parent != null)
            {
                item.Parent.IsChecked = !item.Parent.Children.Any(i => !i.IsChecked);
            }

            Refresh();
        }

        private void AddRemoveCountry(CountriesGroupsDto country)
        {
            if (country.IsChecked)
            {
                var countryExists = Countries?.Count(x => x.Id == country.Id) > 0;
                if (!countryExists)
                {
                    Countries?.Add(new CountryDto { Id = country.Id, Code = country.Code, Name = country.Name });
                }
            }
            else
            {
                var countryToRemove = Countries?.FirstOrDefault(x => x.Id == country.Id);
                if (countryToRemove != null)
                {
                    Countries?.Remove(countryToRemove);
                }
            }
        }
    }
}