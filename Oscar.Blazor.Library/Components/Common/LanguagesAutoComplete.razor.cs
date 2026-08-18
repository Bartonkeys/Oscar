using FluentValidation;
using Microsoft.AspNetCore.Components;
using Oscar.Core.DTOs;
using Oscar.Infrastructure.Features.Channel.Queries;
using Oscar.Infrastructure.Features.Works.Queries;

namespace Oscar.Blazor.Library.Components.Common
{
    public partial class LanguagesAutoComplete
    {
        private List<LanguageDto> _allLanguages;
        private LanguageDto? _selectedLanguage;

        [Parameter]
        public ICollection<LanguageDto> ExistingLanguages { get; set; }

        [Parameter]
        public EventCallback<ICollection<LanguageDto>> ExistingLanguagesChanged { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _allLanguages = (await Mediator.Send(new GetLanguageStaticDataQuery())).Value.OrderBy(x => x.Name).ToList();
            if (ExistingLanguages.Any())
            {
                _allLanguages.RemoveAll(x => ExistingLanguages.Select(c => c.Id).ToList().Contains(x.Id));
                StateHasChanged();
            }
        }

        private async Task<IEnumerable<LanguageDto>> Search(string value, CancellationToken token)
        {
            if (string.IsNullOrEmpty(value))
                return _allLanguages;

            var filteredLanguages = _allLanguages.Where(x =>
            x.Name.StartsWith(value, StringComparison.InvariantCultureIgnoreCase));

            return filteredLanguages;
        }

        private async void AddLanguage()
        {
            if (_selectedLanguage != null && ExistingLanguages.All(v => v.Id != _selectedLanguage.Id))
            {
                var lr = new LanguageDto();
                lr = _selectedLanguage;
                ExistingLanguages.Add(lr);
                _allLanguages.RemoveAll(c => c.Id == _selectedLanguage.Id);
            }
        }

        private async void RemoveLanguage(LanguageDto language)
        {
            var languageToRemove = ExistingLanguages.First(v => v.Id == language.Id);
            if (languageToRemove != null)
            {
                ExistingLanguages.Remove(languageToRemove);
                _allLanguages.Add(language);
            }
        }

        public void onChange(EventArgs args)
        {

        }
    }
}

