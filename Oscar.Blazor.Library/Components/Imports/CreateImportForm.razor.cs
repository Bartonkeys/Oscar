using System.Text;
using BartonKeys.Functional;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using MudBlazor;
using Oscar.Core.DTOs;
using Oscar.Infrastructure.Features.WorksImport.Commands;
using Severity = MudBlazor.Severity;

namespace Oscar.Blazor.Library.Components.Imports
{
    public partial class CreateImportForm
    {
        MudForm form;

        readonly WorksImportRequestValidator orderValidator = new();

        WorksImportRequestViewModel model = new();
        private bool _processing;
        private bool _isUserNameKnown = false;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        [Parameter]
        public List<ClientBasicDto> Clients { get; set; } = new();

        [Parameter]
        public EventCallback OnSubmit { get; set; }

        [Parameter]
        public bool ImportWorks { get; set; } = true;

        [Parameter]
        public string? Title { get; set; } = string.Empty;




        protected override async Task OnInitializedAsync()
        {
            var authstate = await GetAuthenticationStateAsync.GetAuthenticationStateAsync();
            var user = authstate.User;
            if (user != null && user.Identity != null && user.Identity.Name != null)
            {
                model.RequestedBy = user.Identity.Name;
                _isUserNameKnown = true;
                StateHasChanged();
            }
        }

        private async Task Submit()
        {
            _processing = true;
            await form.Validate();

            if (form.IsValid)
            {
                Stream memoryStream = new MemoryStream();
                // Increasing the stream size to 10 MB
                await model.ImportFile.OpenReadStream(10 * 1024 * 1024).CopyToAsync(memoryStream);
                
                var formFile = new FormFile(memoryStream, 0, model.ImportFile.Size, model.ImportFile.Name, model.ImportFile.Name);

                // Check if the file extension is ".xml" or if the content starts with an XML declaration
                //xml file is for Agicoa else excel file is for non agicoa
                //todo: pmalik - remove default value and set it from UI when ready
                bool isXml = Path.GetExtension(model.ImportFile.Name).Equals(".xml", StringComparison.OrdinalIgnoreCase) ||
                             IsXmlFile(memoryStream); 

                var worksImportRequestAddDto = new WorksImportRequestAddDto
                {
                    RequestedBy = model.RequestedBy,
                    ClientId = model.Client.Id,
                    CatalogueId = model.Catalogue?.Id,
                    FormFile = formFile,
                    IsAgicoa = isXml
                };

                if (ImportWorks)
                {
                    var addWorksImportRequestCommand = new AddWorksImportRequestCommand
                    {
                        WorksImportRequestAddDto = worksImportRequestAddDto
                    };

                    var result = await Mediator.Send(addWorksImportRequestCommand, _cancellationTokenSource.Token);

                    await HandleResult(result);
                }
                else
                {
                    var addEpisodesImportRequestCommand = new AddEpisodeImportRequestCommand
                    {
                        WorksImportRequestAddDto = worksImportRequestAddDto
                    };

                    var result = await Mediator.Send(addEpisodesImportRequestCommand, _cancellationTokenSource.Token);

                    await HandleResult(result);
                }
            }
            _processing = false;
        }

        bool IsXmlFile(Stream stream)
        {
            // Reset the position to the beginning of the stream
            stream.Position = 0;

            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, leaveOpen: true))
            {
                // Read the first few characters of the stream
                char[] buffer = new char[5];
                reader.Read(buffer, 0, buffer.Length);

                // Check if the content starts with "<?xml"
                return new string(buffer).StartsWith("<?xml", StringComparison.OrdinalIgnoreCase);
            }
        }

        private async Task HandleResult(Result result)
        {
            if (result.IsSuccess)
            {
                await OnSubmit.InvokeAsync();
                Snackbar.Add("Submitted!", Severity.Success);
            }
            else
            {
                Snackbar.Add(result.Error, Severity.Error, config => { config.VisibleStateDuration = 9000; });
            }
        }

        private async Task<IEnumerable<ClientBasicDto>> SearchClients(string value, CancellationToken token)
        {
            return string.IsNullOrEmpty(value) ? Clients : Clients.Where(x => x.ClientName.Contains(value, StringComparison.InvariantCultureIgnoreCase));
        }

        private void UploadFiles(InputFileChangeEventArgs e)
        {
            model.ImportFile = e.File;
        }

        public class WorksImportRequestValidator : AbstractValidator<WorksImportRequestViewModel>
        {
            public WorksImportRequestValidator()
            {
                RuleFor(x => x.RequestedBy)
                    .NotEmpty()
                    .Length(1, 100);

                RuleFor(x => x.Client)
                    .NotEmpty();

                RuleFor(x => x.ImportFile)
                    .NotNull();
            }

            public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
            {
                var result = await ValidateAsync(ValidationContext<WorksImportRequestViewModel>.CreateWithOptions((WorksImportRequestViewModel)model, x => x.IncludeProperties(propertyName)));
                return result.IsValid ? Array.Empty<string>() : result.Errors.Select(e => e.ErrorMessage);
            };
        }

        public class WorksImportRequestViewModel
        {
            public string RequestedBy { get; set; }
            public ClientBasicDto Client { get; set; }
            public CatalogueDto? Catalogue { get; set; }
            public IBrowserFile ImportFile { get; set; }
        }

    }
}
