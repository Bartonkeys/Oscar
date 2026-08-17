using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Infrastructure.Features.Clients.Queries;
using Oscar.Infrastructure.Features.Report.Queries;
using Oscar.Infrastructure.Features.Report.Services;
using Texnomic.Blazor.JsonViewer;

namespace Oscar.Blazor.Pages
{
    public partial class ReportRun
    {
        [Inject]
        private IJSRuntime JS { get; set; }

        [Parameter]
        public int? ReportId { get; set; }

        public List<ReportDto> ReportList { get; set; } = new();
        public ReportDto SelectedReport { get; set; } = new();

        //TODO: Enums suck - so String List for now - but please fix this Future Michael.
        List<string> WorksTypeStringList = new List<string> { "All", "Season", "Series", "Episode", "Standalone", };
        public string SelectedWorksString { get; set; } = "All";

        public string? ClientSearchText { get; set; }
        public string? WorksTitleSearchText { get; set; }
        public string? ActorFirstNameText { get; set; }
        public string? ActorLastNameText { get; set; }
        public string? DirectorFirstNameText { get; set; }
        public string? DirectorLastNameText { get; set; }
        public string? ProducerFirstNameText { get; set; }
        public string? ProducerLastNameText { get; set; }
        public string? ScreenwriterFirstNameText { get; set; }
        public string? ScreenwriterLastNameText { get; set; }

        public Status? SelectedStatus { get; set; }
        public ClientGrade? SelectedGrade { get; set; }

        private bool _loading { get; set; } = false;
        private bool _isDisabled { get; set; } = false;
        private bool _showFilters { get; set; } = true;

        private int maxJsonStringLengthForOnscreenDisplay = 100000000;
        private Lazy<IEnumerable<ClientDto>> _clients;
        private JsonViewer JsonViewerInstance { get; set; }
        private ClientDto? selectedClient;
        private MudAutocomplete<ClientDto>? clientSelect;

        bool success;
        string[] errors = { };
        MudForm form;

        protected override async Task OnInitializedAsync()
        {
            _loading = true;
            _isDisabled = true;

            var getReportsQuery = new GetReportsQuery();
            ReportList = (await Mediator.Send(getReportsQuery)).Value.Records.ToList();
            if (ReportId != null)
            {
                SelectedReport = ReportList.First(x => x.Id == ReportId);
            }
            else
            {
                if(ReportList.Count > 0)
                SelectedReport = ReportList.OrderBy(x => x.Id).First();

            }
            _clients = new Lazy<IEnumerable<ClientDto>>(() => Mediator.Send(new GetAllClientsQuery()).GetAwaiter().GetResult().Value);
            _loading = false;
            _isDisabled = false;
        }

        private async Task<IEnumerable<ClientDto>> SearchClients(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                selectedClient = null;
                return _clients.Value;
            }

            var filteredClients = _clients.Value.Where(x => x.ClientName.Contains(value, StringComparison.InvariantCultureIgnoreCase));

            return filteredClients;
        }

        private void ResetForm()
        {
            SelectedReport = ReportList.OrderBy(x => x.Id).First();
            SelectedWorksString = "All";
            ClientSearchText = null;
            WorksTitleSearchText = null;
            ClientSearchText = null;
            WorksTitleSearchText = null;
            ActorFirstNameText = null;
            ActorLastNameText = null;
            DirectorFirstNameText = null;
            DirectorLastNameText = null;
            ProducerFirstNameText = null;
            ProducerLastNameText = null;
            ScreenwriterFirstNameText = null;
            ScreenwriterLastNameText = null;
            SelectedStatus = null;
            SelectedGrade = null;

            selectedClient = null;

            _loading = false;
            _isDisabled = false;

            StateHasChanged();
        }

        private async void HandleValidSubmit()
        {
            await form.Validate();

            if (!form.IsValid) return;

            _loading = true;
            _isDisabled = true;

            var selectedReportId = SelectedReport.Id;
            var selectedReport = ReportList.Find(x => x.Id == selectedReportId);
            var clientName = ClientSearchText;

            List<SearchObject?> searchObjects = new List<SearchObject?>
            {

                SelectedWorksString != null  && SelectedWorksString!= "" &&
                !SelectedWorksString.Equals(WorksTypeStringList.ElementAt(0), StringComparison.CurrentCultureIgnoreCase) ?
                new SearchObject("Works", "string", "Discriminator", SelectedWorksString) : null,

                new SearchObject("Clients", "string", "ClientName", selectedClient.ClientName),
                WorksTitleSearchText != null && WorksTitleSearchText != "" ? new SearchObject("WorksTitle", "string", "Title", WorksTitleSearchText) : null,
                ActorFirstNameText != null  && ActorFirstNameText!= "" ? new SearchObject("Actor", "string", "FirstName", ActorFirstNameText) : null,
                ActorLastNameText != null && ActorLastNameText != "" ? new SearchObject("Actor", "string", "LastName", ActorLastNameText) : null,
                DirectorFirstNameText != null  && DirectorFirstNameText!= "" ? new SearchObject("Director", "string", "FirstName", DirectorFirstNameText) : null,
                DirectorLastNameText != null && DirectorLastNameText != "" ? new SearchObject("Director", "string", "LastName", DirectorLastNameText) : null,
                ProducerFirstNameText != null  && ProducerFirstNameText!= "" ? new SearchObject("Producer", "string", "FirstName", ProducerFirstNameText) : null,
                ProducerLastNameText != null && ProducerLastNameText != "" ? new SearchObject("Producer", "string", "LastName", ProducerLastNameText) : null,
                ScreenwriterFirstNameText != null  && ScreenwriterFirstNameText!= "" ? new SearchObject("Screenwriter", "string", "FirstName", ScreenwriterFirstNameText) : null,
                ScreenwriterLastNameText != null && ScreenwriterLastNameText != "" ? new SearchObject("Screenwriter", "string", "LastName", ScreenwriterLastNameText) : null,

                SelectedStatus != null ? new SearchObject("Clients", "number", "Status", ((int)SelectedStatus).ToString()) : null,
                SelectedGrade != null ? new SearchObject("Clients", "number", "ClientGrade", ((int)SelectedGrade).ToString()) : null
            };

            searchObjects.RemoveAll(item => item == null);

            var getReportDataByIdQuery = new AddReportRequestCommand();
            getReportDataByIdQuery.Id = selectedReportId;
            getReportDataByIdQuery.SearchObjects = searchObjects;

            var result = await Mediator.Send(getReportDataByIdQuery);

            if (result.IsSuccess)
                Snackbar.Add("Report successfully queued for processing", Severity.Success);
            else
            {
                Snackbar.Add("There was an error queuing the report", Severity.Error);
                Console.WriteLine("ERROR: " + result.Error);
            }

            _loading = false;
            _isDisabled = false;

            StateHasChanged();
        }

        protected async ValueTask RenderJson(string Json)
        {
            await JsonViewerInstance.Render(Json);
        }
    }
}
