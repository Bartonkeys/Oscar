using BartonKeys.Functional;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Oscar.Core.DTOs;
using Oscar.Infrastructure.Features.Series.Commands;
using Oscar.Infrastructure.Features.Series.Queries;
using Severity = MudBlazor.Severity;

namespace Oscar.Blazor.Library.Components.Works
{
    public partial class WorksForm
    {

        private SeriesDto? _works;
        readonly WorksValidator worksValidator = new();
        MudForm form;

        private bool _isEpisode => _works.Discriminator == "Episode";

        [Parameter]
        public int Id { get; set; } = 0;

        [Parameter]
        public bool Open { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            _works = null;
            if (Id > 0 && Open == true)
            {
                await base.OnParametersSetAsync();
                _works = (await Mediator.Send(new GetSeriesByIdQuery
                {
                    Id = Id
                })).Value;
                StateHasChanged();
            }
        }

        private async Task Submit()
        {
            await form.Validate();

            if (form.IsValid)
            {
                var updateSeriesCommand = new UpdateSeriesCommand
                {
                    Id = _works.Id,
                    SeriesUpdateDto = new SeriesUpdateDto
                    {
                        WorksStatus = _works.WorksStatus,
                        GenreId = _works.GenreId,
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
                        Titles = _works.Titles
                    }
                };

                var result = await Mediator.Send(updateSeriesCommand);

                await HandleResult(result);
            }
        }

        private async Task HandleResult<T>(Result<T> result)
        {
            if (result.IsSuccess)
            {
                Snackbar.Add("Submitted!", Severity.Success);
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
                RuleFor(x => x.DurationMinutes)
                    .NotEmpty()
                    .GreaterThan(0);
                RuleFor(x => x.WorksStatus)
                   .NotNull();
                RuleFor(x => x.GenreId)
                   .NotNull()
                   .GreaterThan(0).WithMessage("Genre is required");
            }

            public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
            {
                var result = await ValidateAsync(ValidationContext<SeriesDto>.CreateWithOptions((SeriesDto)model, x => x.IncludeProperties(propertyName)));
                return result.IsValid ? Array.Empty<string>() : result.Errors.Select(e => e.ErrorMessage);
            };
        }
    }
}
