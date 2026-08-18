using DocumentFormat.OpenXml.Drawing.Diagrams;
using LinqKit;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Oscar.Blazor.Library.Common;
using Oscar.Core.DTOs;
using Oscar.Infrastructure.Features.Common;

namespace Oscar.Blazor.Library.Components.Registration
{
    public partial class ViewRegistrationTree
    {
        [Parameter]
        public HashSet<RegistrationWorksDto>? RegistrationWorks { get; set; }

        [Parameter]
        public HashSet<RegistrationWorksDto>? SelectedValues { get; set; }

        private HashSet<RegistrationWorksDto>? RegistrationWorksCopy = new HashSet<RegistrationWorksDto>();
        private string searchValue = string.Empty;
        private MudTreeView<RegistrationWorksDto>? _registrationWorksTreeView;

        private IReadOnlyCollection<ITreeItemData<RegistrationWorksDto>> RegistrationWorksTree =>
            OscarTreeItem<RegistrationWorksDto>.From(RegistrationWorks, x => x.Children, x => x.Title);


        protected override async Task OnParametersSetAsync()
        {
            RegistrationWorksCopy = CloneHelper.Clone(RegistrationWorks);
        }

        private void Search()
        {
            var registrationWorks = CloneHelper.Clone(RegistrationWorksCopy);
            RegistrationWorks = registrationWorks
                .Where(x => x.Title
                .StartsWith(searchValue, StringComparison.InvariantCultureIgnoreCase))
                .ToHashSet();

            Refresh();
        }

        protected void TreeItemCheckedChanged(RegistrationWorksDto item)
        {
            item.IsChecked = !item.IsChecked;

            AddRemoveRegistration(item, item.IsChecked);

            if (item.HasChild)
            {
                foreach (RegistrationWorksDto child in item.Children)
                {
                    child.IsChecked = item.IsChecked;
                    AddRemoveRegistration(child, child.IsChecked);
                }
            }

            if (item.Parent != null)
            {
                var isChecked = !item.Parent.Children.Any(i => !i.IsChecked);
                SetParentsChecked(item.Parent.Id, isChecked);
            }
        }

        private void SetParentsChecked(int id, bool isChecked)
        {
            RegistrationWorksDto? matchedItem = null;
            RegistrationWorks.ForEach(item => 
                {
                    if (item.Id == id)
                        matchedItem = item;

                    if (matchedItem == null)
                        matchedItem = item.Children.FirstOrDefault(x => x.Id == id);

                    if (matchedItem != null)
                    {
                        matchedItem.IsChecked = isChecked;
                        AddRemoveRegistration(matchedItem, isChecked, false);
                        
                        if (matchedItem.Parent != null)
                        {
                            var found = RegistrationWorks?.FirstOrDefault(x => x.Id == matchedItem.Parent.Id);
                            if (found != null)
                            {
                                found.IsChecked = isChecked;
                                AddRemoveRegistration(found, isChecked, false);
                            }
                        }
                    }
                }
            );
        }

        private void Refresh()
        {
            SetIsChecked(false);
            SetSelectedWorksChecked();
        }

        private void SetIsChecked(bool isChecked)
        {
            ResetAllWorksChecked(RegistrationWorks, isChecked);
            ResetAllWorksChecked(RegistrationWorksCopy, isChecked);

            foreach (var item in _registrationWorksTreeView.Items)
                AddRemoveRegistration(item.Value, isChecked);
        }

        private void ResetAllWorksChecked(HashSet<RegistrationWorksDto> registrationWorks, bool isChecked)
        {
            if (registrationWorks != null)
            {
                foreach (var item in registrationWorks)
                {
                    item.IsChecked = isChecked;
                    if (item.HasChild)
                    {
                        foreach (var child in item.Children)
                            child.IsChecked = isChecked;
                    }
                }
            }
        }

        private void SetSelectedWorksChecked()
        {
            if (SelectedValues != null)
            {
                foreach (var selectedValue in SelectedValues)
                {
                    ResetSelectedWorksChecked(selectedValue, RegistrationWorks);
                    ResetSelectedWorksChecked(selectedValue, RegistrationWorksCopy);
                }
            }
        }

        private void ResetSelectedWorksChecked(RegistrationWorksDto selectedValue, HashSet<RegistrationWorksDto> registrationWorks)
        {
            if (registrationWorks != null)
            {
                foreach (var registrationWork in registrationWorks)
                {
                    if (registrationWork.Id == selectedValue.Id)
                    {
                        registrationWork.IsChecked = true;
                    }

                    SetSelectedChildWorksChecked(selectedValue, registrationWork);
                }
            }
        }

        private void SetSelectedChildWorksChecked(RegistrationWorksDto selectedValue, RegistrationWorksDto registrationWork)
        {
            if (registrationWork.HasChild)
            {
                foreach (RegistrationWorksDto child in registrationWork.Children)
                {
                    if (child.Id == selectedValue.Id)
                    {
                        child.IsChecked = true;
                    }

                    if (child.HasChild)
                        SetSelectedChildWorksChecked(selectedValue, child);
                }
                registrationWork.IsChecked = registrationWork.Children.All(i => i.IsChecked);
            }
        }

        private void AddRemoveRegistration(RegistrationWorksDto registrationWork, bool isChecked, bool addChildren = true)
        {
            if (registrationWork.IsChecked)
            {
                var registrationWorkExists = SelectedValues?.Count(x => x.Id == registrationWork.Id) > 0;
                if (!registrationWorkExists)
                {
                    SelectedValues?.Add(new RegistrationWorksDto { Id = registrationWork.Id, CompactRef = registrationWork.CompactRef, Title = registrationWork.Title });
                }
            }
            else
            {
                var registrationWorkToRemove = SelectedValues?.FirstOrDefault(x => x.Id == registrationWork.Id);
                if (registrationWorkToRemove != null)
                {
                    SelectedValues?.Remove(registrationWorkToRemove);
                }
            }

            if (addChildren)
            {
                if (registrationWork.HasChild)
                {
                    foreach (var child in registrationWork.Children)
                    {
                        child.IsChecked = isChecked;
                        AddRemoveRegistration(child, child.IsChecked);
                    }
                }
            }
        }

    }
}