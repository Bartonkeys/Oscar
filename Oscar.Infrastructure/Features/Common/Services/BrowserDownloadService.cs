using System;
using BartonKeys.Functional;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.JSInterop;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using System.Globalization;
using System.Text;
using Microsoft.Extensions.Logging;
using Oscar.Infrastructure.Features.Common.Contracts;


namespace Oscar.Infrastructure.Features.Common.Services
{
	public class BrowserDownloadService : IBrowserDownload
	{

        private readonly ILogger<ExportService> _logger;
        private readonly IJSRuntime _js;

        public BrowserDownloadService(ILogger<ExportService> logger, IJSRuntime jSRuntime)
		{
            _logger = logger;
            _js = jSRuntime;
        }

        public async Task<Result> ExportWorksAsCsv(IEnumerable<WorksDto> worksDtos, string fileName)
        {
            try
            {
                var csvConfig = new CsvConfiguration(CultureInfo.CurrentCulture)
                {
                    HasHeaderRecord = true,
                    Delimiter = ",",
                    Encoding = Encoding.UTF8
                };

                using (var mem = new MemoryStream())
                using (var writer = new StreamWriter(mem))
                using (var csvWriter = new CsvWriter(writer, csvConfig))
                {
                    csvWriter.Context.RegisterClassMap<WorksDtoMap>();
                    csvWriter.WriteRecords(worksDtos);

                    writer.Flush();
                    writer.BaseStream.Seek(0, SeekOrigin.Begin);

                    var result = Encoding.UTF8.GetString(mem.ToArray());
                    var buffer = Encoding.UTF8.GetBytes(result);
                    var memoryStream = new MemoryStream(buffer);

                    using var streamRef = new DotNetStreamReference(stream: memoryStream);
                    memoryStream.Flush();
                    await _js.InvokeVoidAsync("downloadFileFromStream", fileName, streamRef);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError((int)ImportExportServiceEvent.ExportError, ex.Message);
                return Result.Fail<IEnumerable<WorksDto>>(ex.Message);
            }

            return Result.Ok();
        }
    }
}

