using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Oscar.Core.DTOs;
using Oscar.Infrastructure.Features.Document.Commands;
using Severity = MudBlazor.Severity;
using MudBlazor;

namespace Oscar.Blazor.Library.Components.Common
{
    public partial class DocumentSelect
    {
        private List<DocumentDto> _documents;
        private DocumentDto _documentDto;
        public bool _showCreatePanel = true;
        public IBrowserFile UploadedDocument { get; set; }
        public string DisplayName { get; set; } = "";
        public string FileName { get; set; } = "";
        public string DocumentInfo { get; set; } = "";
        public string UploadedBy { get; set; } = "Unknown";
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        // Currrently set to 2GB
        private const long MaxFileSize = 1024L * 1024L * 1024L * 2L;


        [Parameter]
        public Core.Enums.DocumentType DocumentType { get; set; }

        [Parameter]
        public int OwnerId { get; set; }

        [Parameter]
        public String Header { get; set; } = "";

        [Parameter]
        public ICollection<DocumentDto>? Value { get; set; }

        [Parameter]
        public EventCallback<ICollection<DocumentDto>> ValueChanged { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var authstate = await GetAuthenticationStateAsync.GetAuthenticationStateAsync();
            var user = authstate.User;
            if (user != null && user.Identity != null && user.Identity.Name != null)
            {
                UploadedBy = user.Identity.Name;
            }

            if (Value != null && Value.Count() > 0)
            {
                _documents = Value.ToList();
                StateHasChanged();

            }
            else
            {
                _documents = new List<DocumentDto>();
            }

        }


        private async void ChooseFile(InputFileChangeEventArgs e)
        {
            UploadedDocument = e.File;
            DocumentInfo = $"{UploadedDocument.Size} bytes";
            DisplayName = Path.GetFileNameWithoutExtension(e.File.Name);
            FileName = Path.GetFileName(e.File.Name);
        }

        private async void UploadDocument()
        {
            Stream memoryStream = new MemoryStream();
            await UploadedDocument.OpenReadStream(maxAllowedSize: MaxFileSize).CopyToAsync(memoryStream);
            var formFile = new FormFile(memoryStream, 0, UploadedDocument.Size, UploadedDocument.Name, UploadedDocument.Name);
            _documentDto = new DocumentDto()
            {
                UploadedBy = UploadedBy,
                DocumentType = DocumentType,
                FormFile = formFile,
                OwnerId = OwnerId,
                DisplayName = DisplayName
            };

            var addDocumentCommand = new AddDocumentCommand()
            {
                DocumentDto = _documentDto
            };


            var result = await Mediator.Send(addDocumentCommand, _cancellationTokenSource.Token);

            if (result.IsSuccess)
            {
                Snackbar.Add("Document uploaded", Severity.Success);
                _documents.Add(result.Value);
                FileName = "";
            }
            else
            {
                Snackbar.Add(result.Error, Severity.Error, config => { config.VisibleStateDuration = 9000; });
            }
            ResetForm();
            StateHasChanged();

        }

        private async void RemoveDocument(DocumentDto documentDto)
        {
            IDialogReference? dialog = null;
            DialogResult? dialogResult = null;
            
            dialog = DialogService.Show<ConfirmDialog>("Permanently delete this document?");
            dialogResult = await dialog.Result;
            

            if (dialog == null || !dialogResult.Cancelled)
            {
                var result = await Mediator.Send(new DeleteDocumentCommand { Id = documentDto.Id });

                if (result.IsSuccess)
                {
                    Snackbar.Add("Document deleted", Severity.Success);
                    _documents.Remove(documentDto);
                }
                else
                {
                    Snackbar.Add(result.Error, Severity.Error, config => { config.VisibleStateDuration = 9000; });
                }
                StateHasChanged();
            } 

        }

        private EventCallback showHideCreateForm => new(this, (Action<bool>)((bool isExpanded) => _showCreatePanel = isExpanded));

        private void ResetForm()
        {
            DisplayName = "";
            DocumentInfo = "";
        }
    }
}

