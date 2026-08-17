using Azure.Storage.Blobs;
using BartonKeys.Functional;
using CsvHelper;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Infrastructure.Features.Common.Contracts;
using System.Globalization;

namespace Oscar.Infrastructure.Features.Common.Services
{
    public class ImportService : IImporter
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly ILogger<ImportService> _logger;

        private const string MatchContainerName = "oscar";
        private const string WorksImportContainerName = "oscar-works-import";

        public ImportService(BlobServiceClient blobServiceClient, ILogger<ImportService> logger)
        {
            _blobServiceClient = blobServiceClient;
            _logger = logger;

        }

        public Result<List<MatchTemplateDto>> ImportMatchCsvAsList(string filename)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(MatchContainerName);

                var blobClient = containerClient.GetBlobClient(filename);
                using (var memoryStream = new MemoryStream())
                {
                    using (var reader = new StreamReader(blobClient.DownloadStreaming().Value.Content))
                    {
                        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                        {
                            csv.Context.RegisterClassMap<MatchTemplateDtoMap>();
                            return Result.Ok(csv.GetRecords<MatchTemplateDto>().ToList());
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.LogError((int)ImportExportServiceEvent.ImportError, ex.Message);
                return Result.Fail<List<MatchTemplateDto>>(ex.Message);
            }
        }


        public Result<List<WorksImportDto>> ImportWorksCsvAsList(string filename)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(WorksImportContainerName);

                var blobClient = containerClient.GetBlobClient(filename);
                using (var memoryStream = new MemoryStream())
                {
                    using (var reader = new StreamReader(blobClient.DownloadStreaming().Value.Content))
                    {
                        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                        {
                            csv.Context.RegisterClassMap<WorksImportDtoMap>();
                            return Result.Ok(csv.GetRecords<WorksImportDto>().ToList());
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.LogError((int)ImportExportServiceEvent.ImportError, ex.Message);
                return Result.Fail<List<WorksImportDto>>(ex.Message);
            }
        }

        public Result<byte[]> ImportMatchBlobAsBytes(string filename)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(MatchContainerName);
                var blobClient = containerClient.GetBlobClient(filename);

                if (blobClient.ExistsAsync().Result)
                {
                    using (var ms = new MemoryStream())
                    {
                        blobClient.DownloadTo(ms);
                        return Result.Ok(ms.ToArray());
                    }
                }
                else
                {
                    return Result.Fail<byte[]>(CommandResult.NOTFOUND);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError((int)ImportExportServiceEvent.ImportError, ex.Message);
                return Result.Fail<byte[]>(ex.Message);
            }
        }



        public Result<List<EquivalenceDto>> ImportEquivalenceCsvAsList(string filename)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(ContainerName.EQUIVALENCE);
                CsvHelper.Configuration.CsvConfiguration myConfig = new
                    CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        Delimiter = "\t"
                    };
                var blobClient = containerClient.GetBlobClient(filename);
                using (var memoryStream = new MemoryStream())
                {
                    using (var reader = new StreamReader(blobClient.DownloadStreaming().Value.Content))
                    {
                        using (var csv = new CsvReader(reader, myConfig))
                        {
                            csv.Context.RegisterClassMap<EquivalenceDtoMap>();
                            return Result.Ok(csv.GetRecords<EquivalenceDto>().ToList());
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.LogError((int)ImportExportServiceEvent.ImportError, ex.Message);
                return Result.Fail<List<EquivalenceDto>>(ex.Message);
            }
        }


        public Result<List<ScreenrightsDto>> ImportScreenrightsCsvAsList(string filename)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(ContainerName.SCREENRIGHTS);
                CsvHelper.Configuration.CsvConfiguration myConfig = new
                    CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ","
                };
                var blobClient = containerClient.GetBlobClient(filename);
                using (var memoryStream = new MemoryStream())
                {
                    using (var reader = new StreamReader(blobClient.DownloadStreaming().Value.Content))
                    {
                        using (var csv = new CsvReader(reader, myConfig))
                        {
                            csv.Context.RegisterClassMap<ScreenrightsDtoMap>();
                            return Result.Ok(csv.GetRecords<ScreenrightsDto>().ToList());
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.LogError((int)ImportExportServiceEvent.ImportError, ex.Message);
                return Result.Fail<List<ScreenrightsDto>>(ex.Message);
            }
        }


    }

}


