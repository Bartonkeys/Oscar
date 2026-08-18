using FluentValidation;
using Microsoft.AspNetCore.Components;
using Oscar.Core.DTOs;
using Oscar.Infrastructure.Features.Channel.Queries;
using Oscar.Infrastructure.Features.Works.Queries;

namespace Oscar.Blazor.Library.Components.Rights
{
    public partial class RightsLanguages
    {
        private List<LanguageDto> _languages;
        private LanguageDto? _language;

        [Parameter]
        public String Style { get; set; } = "";

        [Parameter]
        public String Class { get; set; } = "";

        [Parameter]
        public String Header { get; set; } = "";

        [Parameter]
        public String ListLabel { get; set; } = "";

        [Parameter]
        public ICollection<LanguageRightsDto> Value { get; set; }

        [Parameter]
        public EventCallback<ICollection<LanguageRightsDto>> ValueChanged { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            _language = default;
            _languages = (await Mediator.Send(new GetLanguageStaticDataQuery())).Value.OrderBy(x => x.Name).ToList();
            if (Value.Any())
            {
                _languages.RemoveAll(x => Value.Select(c => c.Language.Id).ToList().Contains(x.Id));
            }
            _language = _languages?.FirstOrDefault(x => x.Name == "*");

            StateHasChanged();
        }

        private async Task<IEnumerable<LanguageDto>> Search(string value, CancellationToken token)
        {
            if (string.IsNullOrEmpty(value))
                return _languages;

            var filteredLanguages = _languages.Where(x =>
            x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase));

            return filteredLanguages;
        }

        private async void AddLanguage()
        {
            if (_language != null && !Value.Any(v => v.Language.Id == _language.Id))
            {
                LanguageRightsDto lr = new();
                lr.Language = _language;
                Value.Add(lr);
                _languages.RemoveAll(c => c.Id == _language.Id);
                _language = default;
            }
        }

        private async void RemoveLanguage(LanguageDto language)
        {
            LanguageRightsDto languageToRemove = Value.First(v => v.Language.Id == language.Id);
            if (languageToRemove != null)
            {
                Value.Remove(languageToRemove);
                _languages.Add(language);
                _languages = _languages.OrderBy(x => x.Name).ToList();
                _language = default;
            }
        }

        public void onChange(EventArgs args)
        {

        }
    }
}

