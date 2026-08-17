using System.Linq.Expressions;
using System.Windows.Markup;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Infrastructure.Features.Works.Queries;
using Severity = MudBlazor.Severity;

namespace Oscar.Blazor.Library.Components.Works
{
    public class WorksTitleLanguage
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public LanguageDto? Language { get; set; }
        public TitleType TitleType { get; set; }
    }

    public partial class Titles
    {
        readonly TitleValidator titleValidator = new();
        private List<LanguageDto> _languages;

        [Parameter]
        public Expression<Func<ICollection<WorksTitleDto>>> For { get; set; }

        [Parameter]
        public ICollection<WorksTitleDto> Value { get; set; }

        private List<WorksTitleLanguage> _values;

        private bool _loaded = false;

        private void AlignValues()
            {
            foreach (var item in Values)
            {
                var i = Value.FirstOrDefault(v => v.Id == item.Id);

                if (i != null)
                {
                    i.Title = item.Title;
                    i.LanguageCode = item?.Language?.Name;
                    i.TitleType = item.TitleType;
                }
            }
        }

        public List<WorksTitleLanguage> Values {
            get
            {
                if (_values == null)
                {
                    _values = new List<WorksTitleLanguage>();

                    if (Value != null)
                        foreach (var item in Value)
                            _values.Add(new WorksTitleLanguage { 
                                Id = item.Id, 
                                Title = item.Title, 
                                Language = _languages?.FirstOrDefault(l => l.Name.ToUpper() == item.LanguageCode?.ToUpper()),
                                TitleType = item.TitleType
                            });
                }

                return _values;
            }
            set
            {
                _values = value;
            }
        }

        [Parameter]
        public String Class { get; set; } = "";

        [Parameter]
        public EventCallback<ICollection<WorksTitleDto>> ValueChanged { get; set; }

        [Parameter]
        public bool IsEpisode { get; set; }
        
        public void languageDropdownTextChanged()
        {
            onChange(null);
        }

        public async void onChange(EventArgs args)
        {
            if (_loaded)
                AlignValues();

           // await table.ReloadServerData();
        }

        public async void onTitleChange()
        {
            if (_loaded)
                AlignValues();

            // await table.ReloadServerData();
        }

        private async Task<IEnumerable<LanguageDto>> Search(string value)
        {
            if (string.IsNullOrEmpty(value))
                return _languages;

            var filteredLanguages = _languages.Where(x => x.Description.StartsWith(value, StringComparison.InvariantCultureIgnoreCase));

            return filteredLanguages;
        }

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            _loaded = true;
            return base.OnAfterRenderAsync(firstRender);
        }

        protected override async Task OnInitializedAsync()
        {
            _loaded = false;

            _languages = (await Mediator.Send(new GetLanguageStaticDataQuery { })).Value;

            if (Value == null)
            {
                Value = new List<WorksTitleDto>();
                WorksTitleDto newTitle = new();
                newTitle.LanguageCode = "ENG";
                newTitle.TitleType = TitleType.Main;
                Value.Add(newTitle);
                await ValueChanged.InvokeAsync(Value);
            }

            StateHasChanged();
        }

        private async void addTitle(TitleType titleType)
        {
            int uid = -1;

            if (Value.Any())
            {
                uid = Value.Min(i => i.Id);

                if (uid > -1)
                    uid = 0;

                uid--;
            }

            Value.Add(new WorksTitleDto { LanguageCode = string.Empty, Title = string.Empty, Id = uid, TitleType = titleType });
            Values.Add(new WorksTitleLanguage { Language = null, Title = string.Empty, Id = uid, TitleType = titleType});
            
            await ValueChanged.InvokeAsync(Value);

            StateHasChanged();
        }

        private async void removeTitle(WorksTitleLanguage title)
        {
            if (Values.Count() == 1)
                Snackbar.Add("Work items must have at least one title", Severity.Error);
            else
            {
                var workTitleDto = Value.Single(v => v.Id == title.Id);
                Value.Remove(workTitleDto);
                var workTitleLanguage = Values.Single(v => v.Id == title.Id);
                Values.Remove(workTitleLanguage);
                
                await ValueChanged.InvokeAsync(Value);

                StateHasChanged();
            }
        }
        public class TitleValidator : AbstractValidator<WorksTitleDto>
        {
            public TitleValidator()
            {
                RuleFor(title => title.Title).NotEmpty().WithMessage("Title is required");
            }

            public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
            {
                var modelWTL = (WorksTitleLanguage)model;
                
                var modelDTO = new WorksTitleDto() { Id = modelWTL.Id, LanguageCode = modelWTL?.Language?.Name, Title = modelWTL?.Title };

                var result = await ValidateAsync(ValidationContext<WorksTitleDto>.CreateWithOptions(modelDTO, x => x.IncludeProperties(propertyName)));

                if (result.IsValid)
                    return Array.Empty<string>();
                else
                    return result.Errors.Select(e => e.ErrorMessage);
            };
        }
    }
}
