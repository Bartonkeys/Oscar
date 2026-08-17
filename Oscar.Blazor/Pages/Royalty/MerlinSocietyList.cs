using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using OfficeOpenXml;
using Oscar.Blazor.Components;
using Oscar.Blazor.Library.Common;
using Oscar.Blazor.Library.Components;
using MudBlazor;
using Oscar.Core.DTOs;
using Oscar.Infrastructure.Features.Rights.Commands;
using BartonKeys.Functional;
using Oscar.Infrastructure.Features.Rights.Queries;

namespace Oscar.Blazor.Pages.Royalty
{
    public partial class MerlinSocietyList : OscarComponentBase
    {
        private List<MerlinSocietyDto>? _merlinSocietyItems = new List<MerlinSocietyDto>();
        public string Title { get; set; } = "Merlin Societies";
        private IBrowserFile? selectedFile;
        protected EventConsole Console { get; set; }
        [Inject]
        protected IJSRuntime jsRuntime { get; set; }

        [Inject]
        protected IMediator Mediator { get; set; }

        protected override async void OnInitialized()
        {
            var result = await Mediator.Send(new GetMerlinSocietiesQuery());
            _merlinSocietyItems = result.Value;
        }

        private async Task OnFileChange(InputFileChangeEventArgs e)
        {
            selectedFile = e.File;
        }

        private async Task OnImportFromExcelClick()
        {
            if (selectedFile == null)
            {
                Console?.LogError("Please select file to import");
                return;
            }

            try
            {
                await ImportMerlinSocieties(selectedFile);
            }
            catch (Exception ex)
            {
                Console?.LogException(ex);
            }
        }

        private async Task ImportMerlinSocieties(IBrowserFile file)
        {
            var confirmResult = await DialogService.Show<ConfirmDialog>("This process will add/update merlin societies").Result;
            if (!confirmResult.Canceled)
            {
                await SetStatusAsync(true, "Importing File");
                using var memoryStream = new MemoryStream();
                await file.OpenReadStream(file.Size).CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using var package = new ExcelPackage(memoryStream);
                var sourceWorksheet = package.Workbook.Worksheets[0];

                var merlinSocieties = ImportMerlinSocietiesFromExcel(sourceWorksheet);

                var result = await UpdateMerlinSocieties(merlinSocieties);
                _merlinSocietyItems = result.Value;

                await SetStatusAsync(false, "Importing File");

                if (result.IsSuccess)
                    Snackbar.Add("Merlin societies updated", Severity.Success);
                else
                    Snackbar.Add(result.Error, Severity.Error);
            }
        }

        private async Task OnExportToExcelClick()
        {
            using var outputStream = ExportMerlinSocietiesToExcel();
            using var streamRef = new DotNetStreamReference(stream: outputStream);

            await jsRuntime.InvokeVoidAsync("downloadFileFromStream", "MERLIN SOCIETY CHANNEL LIST.xlsx", streamRef);
        }

        private static List<MerlinSocietyDto> ImportMerlinSocietiesFromExcel(ExcelWorksheet sourceWorksheet)
        {
            var rows = new List<MerlinSocietyDto>();

            int currentRow = 2;

            while (!string.IsNullOrWhiteSpace(sourceWorksheet.Cells[currentRow, 1].Value?.ToString()))
            {
                var newRow = new MerlinSocietyDto();

                newRow.MerlinId = Convert.ToInt32(sourceWorksheet.Cells[currentRow, 1].Value);
                newRow.Merlin_Code = sourceWorksheet.Cells[currentRow, 2].Value?.ToString();
                newRow.Merlin_ChannelName = sourceWorksheet.Cells[currentRow, 3].Value?.ToString();
                newRow.MRIT_Code = sourceWorksheet.Cells[currentRow, 4].Value?.ToString();
                newRow.MRIT_ChannelName = sourceWorksheet.Cells[currentRow, 5].Value?.ToString();
                newRow.AGICOA_Code = sourceWorksheet.Cells[currentRow, 6].Value?.ToString();
                newRow.AGICOA_ChannelName = sourceWorksheet.Cells[currentRow, 7].Value?.ToString();
                newRow.AGICOAGmbh_Code = sourceWorksheet.Cells[currentRow, 8].Value?.ToString();
                newRow.AGICOAGmbh_ChannelName = sourceWorksheet.Cells[currentRow, 9].Value?.ToString();
                newRow.ROVI_Code = sourceWorksheet.Cells[currentRow, 10].Value?.ToString();
                newRow.ROVI_Name = sourceWorksheet.Cells[currentRow, 11].Value?.ToString();
                newRow.TVCountry = sourceWorksheet.Cells[currentRow, 12].Value?.ToString();
                newRow.Countries_CR = sourceWorksheet.Cells[currentRow, 13].Value?.ToString();
                newRow.Countries_BT = sourceWorksheet.Cells[currentRow, 14].Value?.ToString();
                newRow.Countries_EC = sourceWorksheet.Cells[currentRow, 15].Value?.ToString();
                newRow.FilmJus_Code = sourceWorksheet.Cells[currentRow, 16].Value?.ToString();
                newRow.FilmJus_ChannelName = sourceWorksheet.Cells[currentRow, 17].Value?.ToString();
                newRow.ScreenRights_Code = sourceWorksheet.Cells[currentRow, 18].Value?.ToString();
                newRow.ScreenRights_ChannelName = sourceWorksheet.Cells[currentRow, 19].Value?.ToString();
                newRow.PROCIBEL_Code = sourceWorksheet.Cells[currentRow, 20].Value?.ToString();
                newRow.PROCIBEL_ChannelName = sourceWorksheet.Cells[currentRow, 21].Value?.ToString();
                newRow.EGEDA_Code = sourceWorksheet.Cells[currentRow, 22].Value?.ToString();
                newRow.EGEDA_ChannelName = sourceWorksheet.Cells[currentRow, 23].Value?.ToString();
                newRow.FILMKOPI_Code = sourceWorksheet.Cells[currentRow, 24].Value?.ToString();
                newRow.FILMKOPI_ChannelName = sourceWorksheet.Cells[currentRow, 25].Value?.ToString();
                newRow.FRF_Code = sourceWorksheet.Cells[currentRow, 26].Value?.ToString();
                newRow.FRF_ChannelName = sourceWorksheet.Cells[currentRow, 27].Value?.ToString();
                newRow.PROCIREP_Code = sourceWorksheet.Cells[currentRow, 28].Value?.ToString();
                newRow.PROCIREP_ChannelName = sourceWorksheet.Cells[currentRow, 29].Value?.ToString();
                newRow.SIAE_Code = sourceWorksheet.Cells[currentRow, 30].Value?.ToString();
                newRow.SIAE_ChannelName = sourceWorksheet.Cells[currentRow, 31].Value?.ToString();
                newRow.SACEM_Code = sourceWorksheet.Cells[currentRow, 32].Value?.ToString();
                newRow.SACEM_ChannelName = sourceWorksheet.Cells[currentRow, 33].Value?.ToString();
                newRow.SEKAM_Code = sourceWorksheet.Cells[currentRow, 34].Value?.ToString();
                newRow.SEKAM_ChannelName = sourceWorksheet.Cells[currentRow, 35].Value?.ToString();
                newRow.SUISSIMAGE_Code = sourceWorksheet.Cells[currentRow, 36].Value?.ToString();
                newRow.SUISSIMAGE_ChannelName = sourceWorksheet.Cells[currentRow, 37].Value?.ToString();
                newRow.VAM_Code = sourceWorksheet.Cells[currentRow, 38].Value?.ToString();
                newRow.VAM_ChannelName = sourceWorksheet.Cells[currentRow, 39].Value?.ToString();
                newRow.VGF_Code = sourceWorksheet.Cells[currentRow, 40].Value?.ToString();
                newRow.VGF_ChannelName = sourceWorksheet.Cells[currentRow, 41].Value?.ToString();
                newRow.VFF_Code = sourceWorksheet.Cells[currentRow, 42].Value?.ToString();
                newRow.VFF_ChannelName = sourceWorksheet.Cells[currentRow, 43].Value?.ToString();
                newRow.GWFF_Code = sourceWorksheet.Cells[currentRow, 44].Value?.ToString();
                newRow.GWFF_ChannelName = sourceWorksheet.Cells[currentRow, 45].Value?.ToString();
                newRow.ZAPA_Code = sourceWorksheet.Cells[currentRow, 46].Value?.ToString();
                newRow.ZAPA_ChannelName = sourceWorksheet.Cells[currentRow, 47].Value?.ToString();
                newRow.NORWACO_Code = sourceWorksheet.Cells[currentRow, 48].Value?.ToString();
                newRow.NORWACO_ChannelName = sourceWorksheet.Cells[currentRow, 49].Value?.ToString();
                newRow.VIDEMA_Code = sourceWorksheet.Cells[currentRow, 50].Value?.ToString();
                newRow.VIDEMA_ChannelName = sourceWorksheet.Cells[currentRow, 51].Value?.ToString();
                newRow.ANGOA_Code = sourceWorksheet.Cells[currentRow, 52].Value?.ToString();
                newRow.ANGOA_ChannelName = sourceWorksheet.Cells[currentRow, 53].Value?.ToString();
                newRow.Gedipe_Code = sourceWorksheet.Cells[currentRow, 54].Value?.ToString();
                newRow.Gedipe_ChannelName = sourceWorksheet.Cells[currentRow, 55].Value?.ToString();
                newRow.APA_Code = sourceWorksheet.Cells[currentRow, 56].Value?.ToString();
                newRow.APA_ChannelName = sourceWorksheet.Cells[currentRow, 57].Value?.ToString();
                newRow.Conductor_Code = sourceWorksheet.Cells[currentRow, 58].Value?.ToString();
                newRow.Conductor_ChannelName = sourceWorksheet.Cells[currentRow, 59].Value?.ToString();
                newRow.UPFAR_ARGOA_Code = sourceWorksheet.Cells[currentRow, 60].Value?.ToString();
                newRow.UPFAR_ARGOA_ChannelName = sourceWorksheet.Cells[currentRow, 61].Value?.ToString();
                newRow.PRD_Code = sourceWorksheet.Cells[currentRow, 62].Value?.ToString();
                newRow.PRD_ChannelName = sourceWorksheet.Cells[currentRow, 63].Value?.ToString();
                newRow.LITA_Code = sourceWorksheet.Cells[currentRow, 64].Value?.ToString();
                newRow.LITA_ChannelName = sourceWorksheet.Cells[currentRow, 65].Value?.ToString();
                newRow.CMC_Code = sourceWorksheet.Cells[currentRow, 66].Value?.ToString();
                newRow.CMC_ChannelName = sourceWorksheet.Cells[currentRow, 67].Value?.ToString();

                rows.Add(newRow);
                currentRow++;
            }

            return rows;
        }

        private async Task<Result<List<MerlinSocietyDto>>> UpdateMerlinSocieties(List<MerlinSocietyDto> merlinSocieties)
        {
            return await Mediator.Send(new SaveMerlinSocietiesCommand()
            {
                MerlinSocieties = merlinSocieties
            });
        }

        private MemoryStream ExportMerlinSocietiesToExcel()
        {
            var resource = typeof(MerlinSocietyList).Assembly
                            .GetManifestResourceStream("Oscar.Blazor.Pages.Royalty.Templates.MerlinSocietyChannelListTemplate.xlsx");

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage(resource);
            var destinationWorksheet = package.Workbook.Worksheets[0];

            var rowCounter = 2;
            foreach (var row in _merlinSocietyItems)
            {
                destinationWorksheet.Cells[$"A{rowCounter}"].Value = row.MerlinId;
                destinationWorksheet.Cells[$"B{rowCounter}"].Value = row.Merlin_Code;
                destinationWorksheet.Cells[$"C{rowCounter}"].Value = row.Merlin_ChannelName;
                destinationWorksheet.Cells[$"D{rowCounter}"].Value = row.MRIT_Code;
                destinationWorksheet.Cells[$"E{rowCounter}"].Value = row.MRIT_ChannelName;
                destinationWorksheet.Cells[$"F{rowCounter}"].Value = row.AGICOA_Code;
                destinationWorksheet.Cells[$"G{rowCounter}"].Value = row.AGICOA_ChannelName;
                destinationWorksheet.Cells[$"H{rowCounter}"].Value = row.AGICOAGmbh_Code;
                destinationWorksheet.Cells[$"I{rowCounter}"].Value = row.AGICOAGmbh_ChannelName;
                destinationWorksheet.Cells[$"J{rowCounter}"].Value = row.ROVI_Code;
                destinationWorksheet.Cells[$"K{rowCounter}"].Value = row.ROVI_Name;
                destinationWorksheet.Cells[$"L{rowCounter}"].Value = row.TVCountry;
                destinationWorksheet.Cells[$"M{rowCounter}"].Value = row.Countries_CR;
                destinationWorksheet.Cells[$"N{rowCounter}"].Value = row.Countries_BT;
                destinationWorksheet.Cells[$"O{rowCounter}"].Value = row.Countries_EC;
                destinationWorksheet.Cells[$"P{rowCounter}"].Value = row.FilmJus_Code;
                destinationWorksheet.Cells[$"Q{rowCounter}"].Value = row.FilmJus_ChannelName;
                destinationWorksheet.Cells[$"R{rowCounter}"].Value = row.ScreenRights_Code;
                destinationWorksheet.Cells[$"S{rowCounter}"].Value = row.ScreenRights_ChannelName;
                destinationWorksheet.Cells[$"T{rowCounter}"].Value = row.PROCIBEL_Code;
                destinationWorksheet.Cells[$"U{rowCounter}"].Value = row.PROCIBEL_ChannelName;
                destinationWorksheet.Cells[$"V{rowCounter}"].Value = row.EGEDA_Code;
                destinationWorksheet.Cells[$"W{rowCounter}"].Value = row.EGEDA_ChannelName;
                destinationWorksheet.Cells[$"X{rowCounter}"].Value = row.FILMKOPI_Code;
                destinationWorksheet.Cells[$"Y{rowCounter}"].Value = row.FILMKOPI_ChannelName;
                destinationWorksheet.Cells[$"Z{rowCounter}"].Value = row.FRF_Code;
                destinationWorksheet.Cells[$"AA{rowCounter}"].Value = row.FRF_ChannelName;
                destinationWorksheet.Cells[$"AB{rowCounter}"].Value = row.PROCIREP_Code;
                destinationWorksheet.Cells[$"AC{rowCounter}"].Value = row.PROCIREP_ChannelName;
                destinationWorksheet.Cells[$"AD{rowCounter}"].Value = row.SIAE_Code;
                destinationWorksheet.Cells[$"AE{rowCounter}"].Value = row.SIAE_ChannelName;
                destinationWorksheet.Cells[$"AF{rowCounter}"].Value = row.SACEM_Code;
                destinationWorksheet.Cells[$"AG{rowCounter}"].Value = row.SACEM_ChannelName;
                destinationWorksheet.Cells[$"AH{rowCounter}"].Value = row.SEKAM_Code;
                destinationWorksheet.Cells[$"AI{rowCounter}"].Value = row.SEKAM_ChannelName;
                destinationWorksheet.Cells[$"AJ{rowCounter}"].Value = row.SUISSIMAGE_Code;
                destinationWorksheet.Cells[$"AK{rowCounter}"].Value = row.SUISSIMAGE_ChannelName;
                destinationWorksheet.Cells[$"AL{rowCounter}"].Value = row.VAM_Code;
                destinationWorksheet.Cells[$"AM{rowCounter}"].Value = row.VAM_ChannelName;
                destinationWorksheet.Cells[$"AN{rowCounter}"].Value = row.VGF_Code;
                destinationWorksheet.Cells[$"AO{rowCounter}"].Value = row.VGF_ChannelName;
                destinationWorksheet.Cells[$"AP{rowCounter}"].Value = row.VFF_Code;
                destinationWorksheet.Cells[$"AQ{rowCounter}"].Value = row.VFF_ChannelName;
                destinationWorksheet.Cells[$"AR{rowCounter}"].Value = row.GWFF_Code;
                destinationWorksheet.Cells[$"AS{rowCounter}"].Value = row.GWFF_ChannelName;
                destinationWorksheet.Cells[$"AT{rowCounter}"].Value = row.ZAPA_Code;
                destinationWorksheet.Cells[$"AU{rowCounter}"].Value = row.ZAPA_ChannelName;
                destinationWorksheet.Cells[$"AV{rowCounter}"].Value = row.NORWACO_Code;
                destinationWorksheet.Cells[$"AW{rowCounter}"].Value = row.NORWACO_ChannelName;
                destinationWorksheet.Cells[$"AX{rowCounter}"].Value = row.VIDEMA_Code;
                destinationWorksheet.Cells[$"AY{rowCounter}"].Value = row.VIDEMA_ChannelName;
                destinationWorksheet.Cells[$"AZ{rowCounter}"].Value = row.ANGOA_Code;
                destinationWorksheet.Cells[$"BA{rowCounter}"].Value = row.ANGOA_ChannelName;
                destinationWorksheet.Cells[$"BB{rowCounter}"].Value = row.Gedipe_Code;
                destinationWorksheet.Cells[$"BC{rowCounter}"].Value = row.Gedipe_ChannelName;
                destinationWorksheet.Cells[$"BD{rowCounter}"].Value = row.APA_Code;
                destinationWorksheet.Cells[$"BE{rowCounter}"].Value = row.APA_ChannelName;
                destinationWorksheet.Cells[$"BF{rowCounter}"].Value = row.Conductor_Code;
                destinationWorksheet.Cells[$"BG{rowCounter}"].Value = row.Conductor_ChannelName;
                destinationWorksheet.Cells[$"BH{rowCounter}"].Value = row.UPFAR_ARGOA_Code;
                destinationWorksheet.Cells[$"BI{rowCounter}"].Value = row.UPFAR_ARGOA_ChannelName;
                destinationWorksheet.Cells[$"BJ{rowCounter}"].Value = row.PRD_Code;
                destinationWorksheet.Cells[$"BK{rowCounter}"].Value = row.PRD_ChannelName;
                destinationWorksheet.Cells[$"BL{rowCounter}"].Value = row.LITA_Code;
                destinationWorksheet.Cells[$"BM{rowCounter}"].Value = row.LITA_ChannelName;
                destinationWorksheet.Cells[$"BN{rowCounter}"].Value = row.CMC_Code;
                destinationWorksheet.Cells[$"BO{rowCounter}"].Value = row.CMC_ChannelName;


                rowCounter++;
            }

            var ms = new MemoryStream();
            package.SaveAs(ms);
            ms.Position = 0;

            return ms;
        }

    }
}
