using MediatR;
using MudBlazor;
using Oscar.Core.DTOs;
using Oscar.Infrastructure.Features.Series.Queries;
using Oscar.Infrastructure.Features.Report.Queries;
using Oscar.Infrastructure.Features.Report.Commands;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Linq;
using Oscar.Core.Enums;
using Microsoft.Extensions.Options;
using Oscar.Blazor.Library.Components.Clients;
using Oscar.Core.Entities;
using static Oscar.Core.Common.Constants;
using Oscar.Blazor.Library.Components.Works;
using Microsoft.AspNetCore.Components;
using BartonKeys.Functional;

namespace Oscar.Blazor.Pages
{
    public partial class ReportCreate
    {
        private List<ReportFieldDto> selectedReportFields = new();
        private MudTable<ReportFieldDto> table;
        private List<string> reportBaseEntities;
        private List<ReportFieldDto> allReportFields;

        private bool _loading = true;

        private string reportNameString;
        private string baseEntityNameString;

        private string reportFieldBaseEntity;
        private string reportField;

        [Parameter]
        public int ReportId { get; set; }



        protected override async Task OnInitializedAsync()
        {
            _loading = true;
            reportBaseEntities = await GetReportBaseEntities();
            allReportFields = GetAllReportFields();
            _loading = false;

            if(ReportId > 0)
            {
                var result = await Mediator.Send(new GetReportByIdQuery() { Id = ReportId });
                if (result.IsSuccess)
                {
                    var reportToEdit  = result.Value;
                    selectedReportFields = reportToEdit.ReportFields.ToList();
                    reportNameString = reportToEdit.ReportName;
                    baseEntityNameString = reportToEdit.BaseEntityName;

                    StateHasChanged();
                }
            }
        }

        private async Task<List<string>> GetReportBaseEntities()
        {
            var result = await Mediator.Send(new GetReportBaseEntities());
            if (result.IsSuccess)
            {
                return result.Value;

            }
            else
            {
                Snackbar.Add("Failed to load Report data - please refresh this page", Severity.Error);
                return new List<string>();
            }
        }

        // TODO: load from server - POC only
        // NB: Provided the base entity links are all defined in the ReportEntityJoins table then
        // any field from any entity should be available here
        private List<ReportFieldDto> GetAllReportFields()
        {
            return new List<ReportFieldDto>
            {
                new ReportFieldDto ("Clients","ClientName"),
                new ReportFieldDto ("Clients","ClientReference"),
                new ReportFieldDto ("Clients","Status",null, 1),
                new ReportFieldDto ("Clients","ClientGrade", null, 1),
                new ReportFieldDto ("Operators","FullName"),
                new ReportFieldDto ("Clients","ClientType", null, 1),
                new ReportFieldDto ("Clients","IMaestroClientCode"),
                new ReportFieldDto ("Address","AddressLine1"),
                new ReportFieldDto ("Address","AddressLine2"),
                new ReportFieldDto ("Address","AddressLine3"),
                new ReportFieldDto ("Address","AddressLine4"),
                new ReportFieldDto ("Address","PostZipCode"),
                new ReportFieldDto ("Address","Country"),

                new ReportFieldDto ("Clients","GeneralNotes"),
                new ReportFieldDto ("Clients","Email"),

                new ReportFieldDto ("Contract","FirstStartDate"),
                new ReportFieldDto ("Contract","CurrentStartDate"),
                new ReportFieldDto ("Contract","EndDate"),
                new ReportFieldDto ("Contract","AutoRenew"),
                new ReportFieldDto ("Contract","ParentCompany"),
                new ReportFieldDto ("Contract","Notes"),

                new ReportFieldDto ("RightsType","Name"),
                new ReportFieldDto ("Rights","StartOfRight"),
                new ReportFieldDto ("Rights","EndOfRight"),
                new ReportFieldDto ("Rights","StartOfValidity"),
                new ReportFieldDto ("Rights","EndOfValidity"),
                new ReportFieldDto ("Rights","PeriodStart"),
                new ReportFieldDto ("Rights","PeriodEnd"),
                new ReportFieldDto ("Rights","Notations"),
                new ReportFieldDto ("Rights","Percentage"),

                new ReportFieldDto ("Contact","Title"),
                new ReportFieldDto ("Contact","FirstName"),
                new ReportFieldDto ("Contact","LastName"),
                new ReportFieldDto ("Contact","Phone"),
                new ReportFieldDto ("Contact","Mobile"),
                new ReportFieldDto ("Contact","Email"),
                new ReportFieldDto ("Contact","Comments"),

                // Alternate names - not currently in Oscar DB

                new ReportFieldDto ("Society","Name"),
                new ReportFieldDto ("Society","GeneralNotes"),

                // Registration History

                new ReportFieldDto ("Works","Id"),
                new ReportFieldDto ("Works","WorksStatus", null, 1),
                new ReportFieldDto ("Works","DurationMinutes"),
                new ReportFieldDto ("Works","ProductionYear"),
                new ReportFieldDto ("Works","FirstBroadcastYear"),
                new ReportFieldDto ("Works","IMaestroWorkCode"),
                new ReportFieldDto ("Works","AgicoaWorksReference"),
                new ReportFieldDto ("Works","Isan"),
                new ReportFieldDto ("Works","CommissionedWorkStatus", null, 1),
                new ReportFieldDto ("Works","CavcoCode"),
                new ReportFieldDto ("Works","CrtcCode"),
                new ReportFieldDto ("Works","FirstBroadcastYear"),
                new ReportFieldDto ("Works","GeneralNotes"),
                new ReportFieldDto ("Works","Number"),
                new ReportFieldDto ("Works","CompactRef"),

                new ReportFieldDto ("WorksType","Name"),
                new ReportFieldDto ("WorksType","Description"),
                new ReportFieldDto ("WorksSubType","Name"),
                new ReportFieldDto ("WorksSubType","Description"),

                new ReportFieldDto ("Genre","Name"),
                new ReportFieldDto ("Genre","Description"),
                new ReportFieldDto ("GenreSubType","Name"),
                new ReportFieldDto ("GenreSubType","Description"),

                // in conflict (Y/N)

                new ReportFieldDto ("Country","Code"),
                new ReportFieldDto ("Country","Code3A"),
                new ReportFieldDto ("Company","Name"),
                new ReportFieldDto ("Company","Email"),
                new ReportFieldDto ("Producer","FirstName"),
                new ReportFieldDto ("Producer","LastName"),
                new ReportFieldDto ("Director","FirstName"),
                new ReportFieldDto ("Director","LastName"),
                new ReportFieldDto ("Actor","FirstName"),
                new ReportFieldDto ("Actor","LastName"),
                new ReportFieldDto ("Distributor","FirstName"),
                new ReportFieldDto ("Distributor","LastName"),
                new ReportFieldDto ("Screenwriter","FirstName"),
                new ReportFieldDto ("Screenwriter","LastName"),
                new ReportFieldDto ("Scriptwriter","FirstName"),
                new ReportFieldDto ("Scriptwriter","LastName"),
               
                // Conflicts
                
                new ReportFieldDto ("WorksTitle","Title"),

                new ReportFieldDto("Catalogue", "Name"),
                new ReportFieldDto("Catalogue", "IMaestroClientCode"),
                new ReportFieldDto("Catalogue", "Reference"),
                new ReportFieldDto("Catalogue", "GeneralNotes"),

                new ReportFieldDto("OtherName", "Name")
              
                // Catalogue Societies

                // Conflicts

            }.OrderBy(x => x.BaseEntityName).ToList();
        }

        private void AddReportField()
        {
            if (reportFieldBaseEntity == null || reportField == null)
            {
                Snackbar.Add("Please choose a valid Report Field to add", Severity.Info);
                return;
            }

            if (selectedReportFields.Any(item => item.BaseEntityName == reportFieldBaseEntity && item.ReportFieldName == reportField))
            {
                Snackbar.Add("Report Field already added", Severity.Info);
                return;
            }

            var reportFieldToAdd = allReportFields.Where(x => x.ReportFieldName != null && x.BaseEntityName != null
                                                        && x.ReportFieldName.Equals(reportField, StringComparison.OrdinalIgnoreCase)
                                                        && x.BaseEntityName.Equals(reportFieldBaseEntity, StringComparison.OrdinalIgnoreCase)
                                                        ).First();


            selectedReportFields.Add(reportFieldToAdd);
            StateHasChanged();

        }
        private void DeleteReportField(ReportFieldDto reportFieldDto)
        {
            selectedReportFields.Remove(reportFieldDto);
            StateHasChanged();

        }


        private async void CreateUpdateReport()
        {
            //TODO: use standard Form validation
            if (!ValidateReport())
            {
                Snackbar.Add("Report must contain Name, Base Entity and at least one field", Severity.Error);
            }
            else
            {
                _loading = true;

                if (ReportId > 0)
                {

                    ReportDto editReport = new ReportDto();

                    editReport.ReportName = reportNameString;
                    editReport.BaseEntityName = baseEntityNameString;
                    editReport.ReportFields = selectedReportFields;
                    editReport.Id = ReportId;


                    var editReportCommand = new EditReportCommand
                    {
                        ReportEditDto = editReport
                    };
                    var result = await Mediator.Send(editReportCommand);

                    if (result.IsSuccess)
                    {
                        Snackbar.Add($"Report ID {result.Value.Id} updated successfully", Severity.Success);
                        ResetForm();
                        NavigationManager.NavigateTo($"ReportRun/{result.Value.Id}");

                    }
                    else
                    {
                        Snackbar.Add("Report update failed", Severity.Error);

                    }

                }
                else
                {
                    ReportDto newReport = new ReportDto();

                    newReport.ReportName = reportNameString;
                    newReport.BaseEntityName = baseEntityNameString;
                    newReport.ReportFields = selectedReportFields;


                    var addReportCommand = new AddReportCommand
                    {
                        ReportAddDto = newReport
                    };
                    var result = await Mediator.Send(addReportCommand);

                    if (result.IsSuccess)
                    {
                        Snackbar.Add($"Report ID {result.Value.Id} created successfully", Severity.Success);
                        ResetForm();
                        NavigationManager.NavigateTo($"ReportRun/{result.Value.Id}");

                    }
                    else
                    {
                        Snackbar.Add("Report creation failed", Severity.Error);

                    }
                }
                _loading = false;
                StateHasChanged();
            }


        }

        private bool ValidateReport()
        {
            return (selectedReportFields.Count > 0 && baseEntityNameString != null && reportNameString != null);
        }

        private void ResetForm()
        {
            _loading = true;
            selectedReportFields = new();
            reportNameString = null;
            baseEntityNameString = null;
            reportFieldBaseEntity = null;
            reportField = null;
            _loading = false;
            StateHasChanged();
        }

        private void ResetReportFieldSelect()
        {
            reportField = null;
            StateHasChanged();
        }

    }
}
