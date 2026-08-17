using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using OfficeOpenXml;
using Oscar.Blazor.Components;
using Oscar.Blazor.Library.Common;
using Oscar.Infrastructure.Features.Rights.Queries;

namespace Oscar.Blazor.Pages.Royalty
{
    public partial class MaestroPreProcessor : OscarComponentBase
    {
        public string Title { get; set; } = "Society Data File Translator & Collator";
        private IBrowserFile? _selectedFile;
        private string _selectedDateFormatOption = "UK-DateFormat";
        protected EventConsole Console { get; set; }
        [Inject]
        protected IJSRuntime jsRuntime { get; set; }

        private async Task OnFileChange(InputFileChangeEventArgs e)
        {
            _selectedFile = e.File;
        }

        private void OnSelectedDateFormatOptionChanged(string selectedDateFormatOption)
        {
            _selectedDateFormatOption = selectedDateFormatOption;
        }

        private async Task ConvertAndDownload()
        {
            
            if (_selectedFile == null)
            {
                Console?.LogError("Please select source file");
                return;
            }

            await SetStatusAsync(true, "Converting File");
            try
            {
                await ConvertAndDownloadInMaestroFormat(_selectedFile);
            }
            catch (Exception ex)
            {
                Console?.LogException(ex);
            }
            await SetStatusAsync(false, "Converting File");
        }

        private async Task ConvertAndDownloadInMaestroFormat(IBrowserFile file)
        {
            using var memoryStream = new MemoryStream();
            await file.OpenReadStream(file.Size).CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage(memoryStream);
            var sourceWorksheet = package.Workbook.Worksheets[0];

            var maestroSourceRows = CopySourceMaestroData(sourceWorksheet);
            await MergeWithMerlinItems(maestroSourceRows);

            using var outputStream = ConvertToMaestroDestinationFormat(maestroSourceRows);

            using var streamRef = new DotNetStreamReference(stream: outputStream);

            await jsRuntime.InvokeVoidAsync("downloadFileFromStream", $"{file.Name}_Converted.xlsx", streamRef);
        }

        private async Task MergeWithMerlinItems(List<MaestroSourceRow> maestroSourceRows)
        {
            var merlinSocietiesResult = await Mediator.Send(new GetMerlinSocietiesQuery());
            var merlinSocietiesItems = merlinSocietiesResult.Value;

            foreach(var row in maestroSourceRows)
            {
                var matchedItem = merlinSocietiesItems.FirstOrDefault(x => x.AGICOA_Code == row.Channel);
                row.MerlinChannel = matchedItem?.AGICOA_ChannelName;
                row.MerlinCode = matchedItem?.Merlin_Code;
            };
        }

        private MemoryStream ConvertToMaestroDestinationFormat(List<MaestroSourceRow> maestroSourceRows)
        {
            var resource = typeof(MaestroPreProcessor).Assembly
                            .GetManifestResourceStream("Oscar.Blazor.Pages.Royalty.Templates.MaestroGenericInterfaceTemplate.xlsx");

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage(resource);
            var destinationWorksheet = package.Workbook.Worksheets[0];

            var rowCounter = 2;
            foreach (var row in maestroSourceRows)
            {
                string dateValue = string.IsNullOrEmpty(row.Date) ? null : row.Date.Length == 4 ? 
                                        row.Date : DateTime.TryParse(row.Date, out DateTime parsedDate) ? 
                                        _selectedDateFormatOption == "UK-DateFormat"? 
                                        parsedDate.ToString("dd/MM/yyyy"): parsedDate.ToString("MM/dd/yyyy") : row.Date;


                decimal amountValue = 0;
                if (decimal.TryParse(row.Amount, out var amount))
                {
                    amountValue = Math.Round(amount, 2);
                }

                destinationWorksheet.Cells[$"A{rowCounter}"].Value = row.RHnbr;
                destinationWorksheet.Cells[$"B{rowCounter}"].Value = row.RHname;
                destinationWorksheet.Cells[$"C{rowCounter}"].Value = row.DeclNbr;
                destinationWorksheet.Cells[$"D{rowCounter}"].Value = (string.IsNullOrEmpty(row.WrkOrigSerialTitle) ? row.WrkOrigTitle : (row.WrkOrigSerialTitle == row.WrkOrigTitle) ? row.WrkOrigTitle : $"{row.WrkOrigSerialTitle}:{row.WrkOrigTitle}")?.ToUpper();
                destinationWorksheet.Cells[$"E{rowCounter}"].Value = $"CC {row.RHname}".ToUpper();
                destinationWorksheet.Cells[$"F{rowCounter}"].Value = dateValue;
                destinationWorksheet.Cells[$"G{rowCounter}"].Value = null;
                destinationWorksheet.Cells[$"H{rowCounter}"].Value = null;
                destinationWorksheet.Cells[$"I{rowCounter}"].Value = row.RgtPct;
                destinationWorksheet.Cells[$"J{rowCounter}"].Value = row.RgtKnd;
                destinationWorksheet.Cells[$"K{rowCounter}"].Value = amountValue;
                destinationWorksheet.Cells[$"L{rowCounter}"].Value = null;
                destinationWorksheet.Cells[$"M{rowCounter}"].Value = null;
                destinationWorksheet.Cells[$"N{rowCounter}"].Value = row.Country;
                destinationWorksheet.Cells[$"O{rowCounter}"].Value = null;
                destinationWorksheet.Cells[$"P{rowCounter}"].Value = null;
                destinationWorksheet.Cells[$"Q{rowCounter}"].Value = null;
                destinationWorksheet.Cells[$"R{rowCounter}"].Value = null;
                destinationWorksheet.Cells[$"S{rowCounter}"].Value = null;
                destinationWorksheet.Cells[$"T{rowCounter}"].Value = null;
                destinationWorksheet.Cells[$"U{rowCounter}"].Value = null;
                destinationWorksheet.Cells[$"V{rowCounter}"].Value = null;
                destinationWorksheet.Cells[$"W{rowCounter}"].Value = row.BrdTitle;
                destinationWorksheet.Cells[$"X{rowCounter}"].Value = row.WrkOrigTitle;
                destinationWorksheet.Cells[$"Y{rowCounter}"].Value = null;
                destinationWorksheet.Cells[$"Z{rowCounter}"].Value = row.MerlinCode;
                destinationWorksheet.Cells[$"AA{rowCounter}"].Value = row.MerlinChannel;
                destinationWorksheet.Cells[$"AB{rowCounter}"].Value = row.FromCRP;
                destinationWorksheet.Cells[$"AC{rowCounter}"].Value = row.RepartNbr?.PadLeft(4, '0');
                destinationWorksheet.Cells[$"AD{rowCounter}"].Value = row.RHWrkNbr;
                destinationWorksheet.Cells[$"AE{rowCounter}"].Value = null;
                destinationWorksheet.Cells[$"AF{rowCounter}"].Value = null;
                destinationWorksheet.Cells[$"AG{rowCounter}"].Value = null;
                destinationWorksheet.Cells[$"AH{rowCounter}"].Value = row.WrkNbr;
                destinationWorksheet.Cells[$"AI{rowCounter}"].Value = row.Currency;
                destinationWorksheet.Cells[$"AJ{rowCounter}"].Value = $"{row.Type}-{row.Kind}";
                destinationWorksheet.Cells[$"AK{rowCounter}"].Value = row.TransacType;
                destinationWorksheet.Cells[$"AL{rowCounter}"].Value = row.Period;
                destinationWorksheet.Cells[$"AM{rowCounter}"].Value = null;
                destinationWorksheet.Cells[$"AN{rowCounter}"].Value = null;
                destinationWorksheet.Cells[$"AO{rowCounter}"].Value = null;
                destinationWorksheet.Cells[$"AP{rowCounter}"].Value = row.WrkOrigSerialTitle;
                destinationWorksheet.Cells[$"AQ{rowCounter}"].Value = null;
                destinationWorksheet.Cells[$"AR{rowCounter}"].Value = null;
                destinationWorksheet.Cells[$"AS{rowCounter}"].Value = null;
                destinationWorksheet.Cells[$"AT{rowCounter}"].Value = null;
                destinationWorksheet.Cells[$"AU{rowCounter}"].Value = string.IsNullOrEmpty(row.Time) ? null : Convert.ToDateTime(row.Time).TimeOfDay.ToString(@"hh\:mm")?.Replace(":", ""); 
                destinationWorksheet.Cells[$"AV{rowCounter}"].Value = null;
                destinationWorksheet.Cells[$"AW{rowCounter}"].Value = null;
                destinationWorksheet.Cells[$"AX{rowCounter}"].Value = null;
                destinationWorksheet.Cells[$"AY{rowCounter}"].Value = row.Duration;
                destinationWorksheet.Cells[$"AZ{rowCounter}"].Value = null;
                destinationWorksheet.Cells[$"BA{rowCounter}"].Value = null;
                destinationWorksheet.Cells[$"BB{rowCounter}"].Value = row.ISAN;
                destinationWorksheet.Cells[$"BC{rowCounter}"].Value = null;
                destinationWorksheet.Cells[$"BD{rowCounter}"].Value = null;
                rowCounter++;
            }

            var ms = new MemoryStream();
            package.SaveAs(ms);
            ms.Position = 0;

            return ms;
        }

        public static List<MaestroSourceRow> CopySourceMaestroData(ExcelWorksheet sourceWorksheet)
        {
            List<MaestroSourceRow> rows = new List<MaestroSourceRow>();

            int currentRow = 2;

            while (!string.IsNullOrWhiteSpace(sourceWorksheet.Cells[currentRow, 1].Value?.ToString()))
            {
                MaestroSourceRow newRow = new MaestroSourceRow();

                newRow.AGTnbr = sourceWorksheet.Cells[currentRow, 1].Value?.ToString();
                newRow.AGTname = sourceWorksheet.Cells[currentRow, 2].Value?.ToString();
                newRow.RHnbr = sourceWorksheet.Cells[currentRow, 3].Value?.ToString();
                newRow.RHname = sourceWorksheet.Cells[currentRow, 4].Value?.ToString();
                newRow.JNLnbr = sourceWorksheet.Cells[currentRow, 5].Value?.ToString();
                newRow.VoucherNbr = sourceWorksheet.Cells[currentRow, 6].Value?.ToString();
                newRow.Country = sourceWorksheet.Cells[currentRow, 7].Value?.ToString();
                newRow.Period = sourceWorksheet.Cells[currentRow, 8].Value?.ToString();
                newRow.RgtKnd = sourceWorksheet.Cells[currentRow, 9].Value?.ToString();
                newRow.RetransType = sourceWorksheet.Cells[currentRow, 10].Value?.ToString();
                newRow.RepartNbr = sourceWorksheet.Cells[currentRow, 11].Value?.ToString();
                newRow.TransacType = sourceWorksheet.Cells[currentRow, 12].Value?.ToString();
                newRow.Amount = sourceWorksheet.Cells[currentRow, 13].Value?.ToString();
                newRow.Currency = sourceWorksheet.Cells[currentRow, 14].Value?.ToString();
                newRow.RgtPct = sourceWorksheet.Cells[currentRow, 15].Value?.ToString();
                newRow.DeclNbr = sourceWorksheet.Cells[currentRow, 16].Value?.ToString();
                newRow.ISAN = sourceWorksheet.Cells[currentRow, 17].Value?.ToString();
                newRow.WrkNbr = sourceWorksheet.Cells[currentRow, 18].Value?.ToString();
                newRow.WrkEpis = sourceWorksheet.Cells[currentRow, 19].Value?.ToString();
                newRow.SeasonNo = sourceWorksheet.Cells[currentRow, 20].Value?.ToString();
                newRow.RHWrkNbr = sourceWorksheet.Cells[currentRow, 21].Value?.ToString();
                newRow.WrkOrigTitle = sourceWorksheet.Cells[currentRow, 22].Value?.ToString();
                newRow.WrkOrigSerialTitle = sourceWorksheet.Cells[currentRow, 23].Value?.ToString();
                newRow.BrdNbr = sourceWorksheet.Cells[currentRow, 24].Value?.ToString();
                newRow.BrdExtRef = sourceWorksheet.Cells[currentRow, 25].Value?.ToString();
                newRow.Channel = sourceWorksheet.Cells[currentRow, 26].Value?.ToString();
                newRow.Date = sourceWorksheet.Cells[currentRow, 27].Value?.ToString();
                newRow.Time = sourceWorksheet.Cells[currentRow, 28].Value?.ToString();
                newRow.Duration = sourceWorksheet.Cells[currentRow, 29].Value?.ToString();
                newRow.BrdLng = sourceWorksheet.Cells[currentRow, 30].Value?.ToString();
                newRow.Type = sourceWorksheet.Cells[currentRow, 31].Value?.ToString();
                newRow.Kind = sourceWorksheet.Cells[currentRow, 32].Value?.ToString();
                newRow.BrdTitle = sourceWorksheet.Cells[currentRow, 33].Value?.ToString();
                newRow.OrigTitle = sourceWorksheet.Cells[currentRow, 34].Value?.ToString();
                newRow.Remarks = sourceWorksheet.Cells[currentRow, 35].Value?.ToString();
                newRow.FDPayment = sourceWorksheet.Cells[currentRow, 36].Value?.ToString();
                newRow.FromCRP = sourceWorksheet.Cells[currentRow, 37].Value?.ToString();

                rows.Add(newRow);
                currentRow++;
            }

            return rows;
        }
    }
}
