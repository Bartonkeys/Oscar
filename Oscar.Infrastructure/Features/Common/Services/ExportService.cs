using Azure.Storage.Blobs;
using BartonKeys.Functional;
using CsvHelper;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Infrastructure.Features.Common.Contracts;
using System.Globalization;
using Azure;
using Azure.Storage.Blobs.Models;
using System.Xml;
using System.Xml.Serialization;
using CsvHelper.Configuration;
using Microsoft.JSInterop;
using System.Text;
using ClosedXML.Excel;
using Oscar.Core.Schemas;
using Oscar.Infrastructure.Features.Report.Services;
using static Oscar.Core.DTOs.WorksDtoMap;

namespace Oscar.Infrastructure.Features.Common.Services
{
    public class ExportService : IExporter
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly ILogger<ExportService> _logger;


        public ExportService(BlobServiceClient blobServiceClient, ILogger<ExportService> logger)
        {
            _blobServiceClient = blobServiceClient;
            _logger = logger;
        }

        public Result ExportListAsCsv(IEnumerable<MatchTemplateResultsDto> matchTemplateDtos, string fileName)
        {
            Response<BlobContentInfo> response;
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient("oscar");

                BlobClient blobClient = containerClient.GetBlobClient(fileName);

                using (var memoryStream = new MemoryStream())
                {
                    using (var writer = new StreamWriter(memoryStream))
                    {
                        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                        {
                            csv.Context.RegisterClassMap<MatchTemplateResultsMap>();
                            csv.WriteRecords(matchTemplateDtos);
                            writer.Flush();

                            writer.BaseStream.Seek(0, SeekOrigin.Begin);

                            try
                            {
                                response = blobClient.Upload(writer.BaseStream);
                            }
                            catch (Azure.RequestFailedException ex)
                            {
                                _logger.LogError((int)ImportExportServiceEvent.ExportError, ex.Message);
                                throw;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError((int)ImportExportServiceEvent.ExportError, ex.Message);
                return Result.Fail<IEnumerable<MatchTemplateResultsDto>>(ex.Message);
            }

            return Result.Ok();
        }

        public Result ExportWorksImportAsCsv(IEnumerable<WorksImportDto> worksImportDtos, string fileName)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient("oscar-works-import");

                BlobClient blobClient = containerClient.GetBlobClient(fileName);
                using (var memoryStream = new MemoryStream())
                {
                    using (var writer = new StreamWriter(memoryStream))
                    {
                        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                        {
                            csv.Context.RegisterClassMap<WorksImportDtoMap>();
                            csv.WriteRecords(worksImportDtos);
                            writer.Flush();

                            writer.BaseStream.Seek(0, SeekOrigin.Begin);

                            blobClient.Upload(writer.BaseStream);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError((int)ImportExportServiceEvent.ExportError, ex.Message);
                return Result.Fail(ex.Message);
            }

            return Result.Ok();
        }

        public async Task<Result<string>> ExportReportsAsCsv(ReportDataDto reportDataDto, string fileName)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient("oscar-reporting");
                var blobClient = containerClient.GetBlobClient(fileName);

                var csvString = ReportHelperService.ConvertJsonToCsv(reportDataDto.ReportData[0].ToString());
                if (csvString != null)
                {
                    var buffer = Encoding.UTF8.GetBytes(csvString);
                    var memoryStream = new MemoryStream(buffer);

                    using var streamRef = new DotNetStreamReference(stream: memoryStream);
                    memoryStream.Flush();

                    memoryStream.Seek(0, SeekOrigin.Begin);

                    await blobClient.DeleteIfExistsAsync();
                    await blobClient.UploadAsync(memoryStream);
                }

                return Result.Ok(blobClient.Uri.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError((int)ImportExportServiceEvent.ExportError, ex.Message);
                return Result.Fail<string>(ex.Message);
            }
        }


        public Result ExportRegistrationsAsCsv(IEnumerable<RegistrationCreateDto> registrationCreateDtos, string fileName)
        {
            Response<BlobContentInfo> response;
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient("oscar-registrations");

                BlobClient blobClient = containerClient.GetBlobClient(fileName);

                using (var memoryStream = new MemoryStream())
                {
                    using (var writer = new StreamWriter(memoryStream))
                    {
                        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                        {
                            csv.Context.RegisterClassMap<RegistrationCreateMap>();
                            csv.WriteRecords(registrationCreateDtos);
                            writer.Flush();

                            writer.BaseStream.Seek(0, SeekOrigin.Begin);

                            try
                            {
                                response = blobClient.Upload(writer.BaseStream);
                            }
                            catch (Azure.RequestFailedException ex)
                            {
                                _logger.LogError((int)ImportExportServiceEvent.ExportError, ex.Message);
                                throw;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError((int)ImportExportServiceEvent.ExportError, ex.Message);
                return Result.Fail<IEnumerable<RegistrationCreateDto>>(ex.Message);
            }

            return Result.Ok();
        }


        public Result ExportRegistrationsAsXml(IEnumerable<RegistrationCreateDto> registrationCreateDtos, string fileName)
        {
            
            Response<BlobContentInfo> response;
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient("oscar-registrations");
                var registrations = registrationCreateDtos.ToList();
                BlobClient blobClient = containerClient.GetBlobClient(fileName);
                XmlSerializer serializer = new XmlSerializer(registrations.GetType());
                using (var memoryStream = new MemoryStream())
                {
                    using (var writer = new StreamWriter(memoryStream))
                    {
                            serializer.Serialize(writer, registrations);
                            writer.Flush();

                            writer.BaseStream.Seek(0, SeekOrigin.Begin);

                            try
                            {
                                response = blobClient.Upload(writer.BaseStream);
                            }
                            catch (Azure.RequestFailedException ex)
                            {
                                _logger.LogError((int)ImportExportServiceEvent.ExportError, ex.Message);
                                throw;
                            }
                            finally
                            {
                                writer.Close();

                            }
                        
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError((int)ImportExportServiceEvent.ExportError, ex.Message);
                return Result.Fail<IEnumerable<RegistrationCreateDto>>(ex.Message);
            }

            return Result.Ok();
        }


        public Result ExportRegistrations(IRegistrationWorksScreenrights screenrightsWri)
        {

            Response<BlobContentInfo> response;
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient("oscar-registrations");

                var blobClient = containerClient.GetBlobClient(screenrightsWri.FileName);
                var serializer = new XmlSerializer(screenrightsWri.GetType());

                using var memoryStream = new MemoryStream();
                using var writer = new StreamWriter(memoryStream);
                using var xmlTextWriter = new XmlTextWriter(writer);
                xmlTextWriter.Formatting = Formatting.Indented;

                var namespaces = new XmlSerializerNamespaces();
                namespaces.Add("", "");

                serializer.Serialize(xmlTextWriter, screenrightsWri, namespaces);
                writer.Flush();

                writer.BaseStream.Seek(0, SeekOrigin.Begin);

                try
                {
                    if (blobClient.Exists())
                        blobClient.Delete();

                    blobClient.Upload(writer.BaseStream);
                }
                catch (Azure.RequestFailedException ex)
                {
                    _logger.LogError((int)ImportExportServiceEvent.ExportError, ex.Message);
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError((int)ImportExportServiceEvent.ExportError, ex.Message);
                    throw;
                }
                finally
                {
                    writer.Close();

                }
            }
            catch (Exception ex)
            {
                _logger.LogError((int)ImportExportServiceEvent.ExportError, ex.Message);
                return Result.Fail<IEnumerable<RegistrationCreateDto>>(ex.Message);
            }

            return Result.Ok();
        }

        public Result ExportRegistrations(IRegistration agicoaWri)
        {

            Response<BlobContentInfo> response;
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient("oscar-registrations");

                var blobClient = containerClient.GetBlobClient(agicoaWri.FileName);
                var serializer = new XmlSerializer(agicoaWri.GetType());

                using var memoryStream = new MemoryStream();

                // use UTF-8 without BOM encoding to match with the specification mentioned in OTP-146
                using var writer = new StreamWriter(memoryStream, new UTF8Encoding(false));

                using var xmlTextWriter = new XmlTextWriter(writer);
                xmlTextWriter.Formatting = Formatting.Indented;

                var namespaces = new XmlSerializerNamespaces();
                namespaces.Add("", "");

                serializer.Serialize(xmlTextWriter, agicoaWri, namespaces);
                writer.Flush();

                writer.BaseStream.Seek(0, SeekOrigin.Begin);

                try
                {
                    if (blobClient.Exists()) 
                        blobClient.Delete();

                    blobClient.Upload(writer.BaseStream);
                }
                catch (Azure.RequestFailedException ex)
                {
                    _logger.LogError((int)ImportExportServiceEvent.ExportError, ex.Message);
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError((int)ImportExportServiceEvent.ExportError, ex.Message);
                    throw;
                }
                finally
                {
                    writer.Close();

                }
            }
            catch (Exception ex)
            {
                _logger.LogError((int)ImportExportServiceEvent.ExportError, ex.Message);
                return Result.Fail<IEnumerable<RegistrationCreateDto>>(ex.Message);
            }

            return Result.Ok();
        }

        public Result ExportRegistrations(RegistrationWorksCCCDto registrationWorksCcc)
        {
            var resource = typeof(ExportService).Assembly
                .GetManifestResourceStream("Oscar.Infrastructure.Features.Registration.Templates.CCCTemplate.xlsm");

            var containerClient = _blobServiceClient.GetBlobContainerClient("oscar-registrations");
            BlobClient blobClient = containerClient.GetBlobClient(registrationWorksCcc.FileName);

            using var package = new XLWorkbook(resource);
            var claimantInformation = package.Worksheet(1);
            var titleRegistrations = package.Worksheet(2);
            titleRegistrations.ShowGridLines = false;

            claimantInformation.Cell($"B{8}").Value = registrationWorksCcc.ClaimantName;
            titleRegistrations.Cell($"E{3}").Value = registrationWorksCcc.RoyaltyPeriod;
            titleRegistrations.Cell($"E{4}").Value = registrationWorksCcc.ClaimantName;
            titleRegistrations.Cell($"E{6}").Value = registrationWorksCcc.ReturnDate;

            var rowCounter = 10;
            foreach (var cccRow in registrationWorksCcc.Rows)
            {
                if(rowCounter > 10)
                    CopyRow(titleRegistrations, 10, rowCounter);

                titleRegistrations.Cell($"B{rowCounter}").SetValue(cccRow.ClaimantId);
                titleRegistrations.Cell($"C{rowCounter}").SetValue(cccRow.ClaimantInternalReferenceNumber);
                titleRegistrations.Cell($"D{rowCounter}").Value = cccRow.OwnershipPercentage;
                titleRegistrations.Cell($"E{rowCounter}").Value = cccRow.Title;
                titleRegistrations.Cell($"F{rowCounter}").Value = cccRow.EpisodeTitle;
                titleRegistrations.Cell($"G{rowCounter}").Value = cccRow.Genre;
                titleRegistrations.Cell($"H{rowCounter}").Value = cccRow.CopyrightYear;
                titleRegistrations.Cell($"I{rowCounter}").Value = cccRow.Country;
                titleRegistrations.Cell($"J{rowCounter}").Value = cccRow.Syndication;
                titleRegistrations.Cell($"K{rowCounter}").Value = cccRow.Duration;
                titleRegistrations.Cell($"L{rowCounter}").SetValue(cccRow.StartDate);
                titleRegistrations.Cell($"M{rowCounter}").SetValue(cccRow.EndDate);
                titleRegistrations.Cell($"N{rowCounter}").Value = cccRow.Broadcast;
                titleRegistrations.Cell($"O{rowCounter}").Value = cccRow.PrincipalCast;

                rowCounter++;
            }

            using var ms = new MemoryStream();
            package.SaveAs(ms);
            ms.Position = 0;

            if (blobClient.Exists())
                blobClient.Delete();

            blobClient.Upload(ms);

            return Result.Ok();
        }

        public Result ExportRegistrations(RegistrationWorksCMCDto registrationWorksCmc)
        {
            var resource = typeof(ExportService).Assembly
               .GetManifestResourceStream("Oscar.Infrastructure.Features.Registration.Templates.CMCTemplate.xlsx");

            var containerClient = _blobServiceClient.GetBlobContainerClient("oscar-registrations");
            var blobClient = containerClient.GetBlobClient(registrationWorksCmc.FileName);

            using var package = new XLWorkbook(resource);
            var worksheet = package.Worksheet(1);
            worksheet.ShowGridLines = true;

            var rowCounter = 2;
            foreach (var cmcRow in registrationWorksCmc.Rows)
            {
                if (rowCounter > 2)
                    CopyRow(worksheet, 2, rowCounter);

                worksheet.Cell($"A{rowCounter}").Value = cmcRow.RHID;
                worksheet.Cell($"B{rowCounter}").Value = cmcRow.CMCID;
                worksheet.Cell($"C{rowCounter}").Value = cmcRow.Type;
                worksheet.Cell($"D{rowCounter}").Value = cmcRow.Genre;
                worksheet.Cell($"E{rowCounter}").Value = cmcRow.Duration;
                worksheet.Cell($"F{rowCounter}").Value = cmcRow.ISAN;
                worksheet.Cell($"G{rowCounter}").Value = cmcRow.EIDR;
                worksheet.Cell($"H{rowCounter}").Value = cmcRow.YearOfProduction;
                worksheet.Cell($"I{rowCounter}").Value = cmcRow.OriginalTitleLanguage;
                worksheet.Cell($"J{rowCounter}").Value = cmcRow.OriginalTitle;
                worksheet.Cell($"K{rowCounter}").Value = cmcRow.AlternativeTitleLanguage;
                worksheet.Cell($"L{rowCounter}").Value = cmcRow.AlternativeTitle;
                worksheet.Cell($"M{rowCounter}").Value = cmcRow.SerialOriginalTitleLanguage;
                worksheet.Cell($"N{rowCounter}").Value = cmcRow.SerialOriginalTitle;
                worksheet.Cell($"O{rowCounter}").Value = cmcRow.SerialAlternativeTitleLanguage;
                worksheet.Cell($"P{rowCounter}").Value = cmcRow.SerialAlternativeTitle;
                worksheet.Cell($"Q{rowCounter}").Value = cmcRow.SerialLevel;
                worksheet.Cell($"R{rowCounter}").Value = cmcRow.SeasonNumber;
                worksheet.Cell($"S{rowCounter}").Value = cmcRow.EpisodeNumber;
                worksheet.Cell($"T{rowCounter}").Value = cmcRow.Director1FirstName; 
                worksheet.Cell($"U{rowCounter}").Value = cmcRow.Director1LastName;
                worksheet.Cell($"V{rowCounter}").Value = cmcRow.Director2FirstName;
                worksheet.Cell($"W{rowCounter}").Value = cmcRow.Director2LastName;
                worksheet.Cell($"X{rowCounter}").Value = cmcRow.Writer1FirstName;
                worksheet.Cell($"Y{rowCounter}").Value = cmcRow.Writer1LastName;
                worksheet.Cell($"Z{rowCounter}").Value = cmcRow.Writer2FirstName;
                worksheet.Cell($"AA{rowCounter}").Value = cmcRow.Writer2LastName;
                worksheet.Cell($"AB{rowCounter}").Value = cmcRow.Actor1FirstName;
                worksheet.Cell($"AC{rowCounter}").Value = cmcRow.Actor1LastName;
                worksheet.Cell($"AD{rowCounter}").Value = cmcRow.Actor2FirstName;
                worksheet.Cell($"AE{rowCounter}").Value = cmcRow.Actor2LastName;
                worksheet.Cell($"AF{rowCounter}").Value = cmcRow.Actor3FirstName;
                worksheet.Cell($"AG{rowCounter}").Value = cmcRow.Actor3LastName;
                worksheet.Cell($"AH{rowCounter}").Value = cmcRow.ProductionCountry1;
                worksheet.Cell($"AI{rowCounter}").Value = cmcRow.ProductionCountry2;
                worksheet.Cell($"AJ{rowCounter}").Value = cmcRow.ProductionCountry3;
                worksheet.Cell($"AK{rowCounter}").Value = cmcRow.OriginalLanguage;
                worksheet.Cell($"AL{rowCounter}").Value = cmcRow.ProductionCompany1;
                worksheet.Cell($"AM{rowCounter}").Value = cmcRow.ProductionCompany2;
                worksheet.Cell($"AN{rowCounter}").Value = cmcRow.ProductionCompany3;
                worksheet.Cell($"AO{rowCounter}").Value = cmcRow.Delete;
                worksheet.Cell($"AP{rowCounter}").Value = cmcRow.Tags;
                worksheet.Cell($"AQ{rowCounter}").Value = cmcRow.RightsStartDate;
                worksheet.Cell($"AR{rowCounter}").Value = cmcRow.RightsEndDate;

                rowCounter++;
            }

            using var ms = new MemoryStream();
            package.SaveAs(ms);
            ms.Position = 0;

            if (blobClient.Exists())
                blobClient.Delete();

            blobClient.Upload(ms);

            return Result.Ok();
        }

        public Result ExportRegistrations(RegistrationWorksMPLCDto registrationWorksMplc)
        {
            var resource = typeof(ExportService).Assembly
               .GetManifestResourceStream("Oscar.Infrastructure.Features.Registration.Templates.MPLCTemplate.xlsx");

            var containerClient = _blobServiceClient.GetBlobContainerClient("oscar-registrations");
            var blobClient = containerClient.GetBlobClient(registrationWorksMplc.FileName);

            using var package = new XLWorkbook(resource);
            var worksheet = package.Worksheet(1);
            worksheet.ShowGridLines = true;

            var rowCounter = 2;
            foreach (var mplcRow in registrationWorksMplc.Rows)
            {
                if (rowCounter > 2)
                    CopyRow(worksheet, 2, rowCounter);

                worksheet.Cell($"A{rowCounter}").Value = mplcRow.CompactRef;
                worksheet.Cell($"B{rowCounter}").Value = mplcRow.Title;
                worksheet.Cell($"C{rowCounter}").Value = mplcRow.WorkType;
                worksheet.Cell($"D{rowCounter}").Value = string.Join(",", new[] { mplcRow.ProductionCountry1, mplcRow.ProductionCountry2, mplcRow.ProductionCountry3 }.Where(country => !string.IsNullOrEmpty(country)));
                worksheet.Cell($"E{rowCounter}").Value = mplcRow.OwningClient;
                worksheet.Cell($"F{rowCounter}").Value = string.Join(",", new[]
                                                         {
                                                           $"{mplcRow.Director1FirstName} {mplcRow.Director1LastName}".Trim(),
                                                           $"{mplcRow.Director2FirstName} {mplcRow.Director2LastName}".Trim(),
                                                           $"{mplcRow.Director3FirstName} {mplcRow.Director3LastName}".Trim()
                                                         }.Where(director => !string.IsNullOrWhiteSpace(director)));
                worksheet.Cell($"G{rowCounter}").Value = mplcRow.YearOfProduction;
                rowCounter++;
            }

            using var ms = new MemoryStream();
            package.SaveAs(ms);
            ms.Position = 0;

            if (blobClient.Exists())
                blobClient.Delete();

            blobClient.Upload(ms);

            return Result.Ok();
        }

        public Result ExportRegistrations(RegistrationWorksCRCDto registrationWorksCrc)
        {
            var resource = typeof(ExportService).Assembly
              .GetManifestResourceStream("Oscar.Infrastructure.Features.Registration.Templates.CRCTemplate.xlsx");

            var containerClient = _blobServiceClient.GetBlobContainerClient("oscar-registrations");
            var blobClient = containerClient.GetBlobClient(registrationWorksCrc.FileName);

            using var package = new XLWorkbook(resource);
            var worksheet = package.Worksheet(1);
            worksheet.ShowGridLines = true;


            worksheet.Cell($"A1").Value = $"Claimant: Compact Collections Ltd for {registrationWorksCrc.ClientName}";

            var rowCounter = 4;
            foreach (var cmcRow in registrationWorksCrc.Rows)
            {
                if (rowCounter > 4)
                    CopyRow(worksheet, 4, rowCounter);

                worksheet.Cell($"A{rowCounter}").DataType = XLDataType.Text;
                worksheet.Cell($"A{rowCounter}").SetValue(cmcRow.CompactRef);
                worksheet.Cell($"B{rowCounter}").Value = cmcRow.Name;
                worksheet.Cell($"C{rowCounter}").Value = cmcRow.FirstStartDate;
                worksheet.Cell($"D{rowCounter}").Value = cmcRow.EndDate;
                worksheet.Cell($"E{rowCounter}").Value = cmcRow.OriginalTitle;
                worksheet.Cell($"F{rowCounter}").Value = cmcRow.EpisodeTitle;
                worksheet.Cell($"G{rowCounter}").Value = cmcRow.SeasonCount;
                worksheet.Cell($"H{rowCounter}").Value = cmcRow.EpisodeCount;
                worksheet.Cell($"I{rowCounter}").Value = cmcRow.AltTitles;
                worksheet.Cell($"J{rowCounter}").Value = cmcRow.TitleType;
                worksheet.Cell($"K{rowCounter}").Value = cmcRow.WorkType;
                worksheet.Cell($"L{rowCounter}").Value = cmcRow.ProductionCompanies;
                worksheet.Cell($"M{rowCounter}").Value = cmcRow.Directors;
                worksheet.Cell($"N{rowCounter}").Value = cmcRow.Actors;
                worksheet.Cell($"O{rowCounter}").Value = cmcRow.ProductionYear;
                worksheet.Cell($"P{rowCounter}").Value = cmcRow.ProductionCountries;
                worksheet.Cell($"Q{rowCounter}").Value = cmcRow.Duration;
                worksheet.Cell($"R{rowCounter}").Value = cmcRow.RightsStr;

                rowCounter++;
            }

            using var ms = new MemoryStream();
            package.SaveAs(ms);
            ms.Position = 0;

            if (blobClient.Exists())
                blobClient.Delete();

            blobClient.Upload(ms);

            return Result.Ok();
        }

        public Result ExportRegistrations(RegistrationWorksEGEDADto registrationWorksEgeda)
        {
            var resource = typeof(ExportService).Assembly
              .GetManifestResourceStream("Oscar.Infrastructure.Features.Registration.Templates.EGEDATemplate.xlsx");

            var containerClient = _blobServiceClient.GetBlobContainerClient("oscar-registrations");
            var blobClient = containerClient.GetBlobClient(registrationWorksEgeda.FileName);

            using var package = new XLWorkbook(resource);
            var worksheet = package.Worksheet(1);
            worksheet.ShowGridLines = true;


            worksheet.Cell($"A1").Value = $"Compact Collections - EGEDA: {registrationWorksEgeda.ClientName}";

            var rowCounter = 4;
            foreach (var egedaRow in registrationWorksEgeda.Rows)
            {
                if (rowCounter > 4)
                    CopyRow(worksheet, 4, rowCounter);

                worksheet.Cell($"A{rowCounter}").DataType = XLDataType.Text;
                worksheet.Cell($"A{rowCounter}").SetValue(egedaRow.CompactRef);
                worksheet.Cell($"B{rowCounter}").Value = egedaRow.TitleLanguages;
                worksheet.Cell($"C{rowCounter}").Value = egedaRow.Titles;
                worksheet.Cell($"D{rowCounter}").Value = egedaRow.SeasonNo;
                worksheet.Cell($"E{rowCounter}").Value = egedaRow.EpisodeNo;
                worksheet.Cell($"F{rowCounter}").Value = egedaRow.Duration;
                worksheet.Cell($"G{rowCounter}").Value = egedaRow.WorkType;
                worksheet.Cell($"H{rowCounter}").Value = egedaRow.Genre;
                worksheet.Cell($"I{rowCounter}").Value = egedaRow.YearOfProd;
                worksheet.Cell($"J{rowCounter}").Value = egedaRow.FirstShowing;
                worksheet.Cell($"K{rowCounter}").DataType = XLDataType.Text;
                worksheet.Cell($"K{rowCounter}").SetValue(egedaRow.ISANNo);
                worksheet.Cell($"L{rowCounter}").Value = egedaRow.Colour;
                worksheet.Cell($"M{rowCounter}").Value = egedaRow.BlackAndWhite;
                worksheet.Cell($"N{rowCounter}").Value = egedaRow.Silent;
                worksheet.Cell($"O{rowCounter}").Value = egedaRow.CountriesOfProduction;
                worksheet.Cell($"P{rowCounter}").Value = egedaRow.OriginalLanguages;
                worksheet.Cell($"Q{rowCounter}").Value = egedaRow.Directors;
                worksheet.Cell($"R{rowCounter}").Value = egedaRow.Actors;
                worksheet.Cell($"S{rowCounter}").Value = egedaRow.Producers;
                worksheet.Cell($"T{rowCounter}").Value = egedaRow.Writers;
                worksheet.Cell($"U{rowCounter}").Value = egedaRow.ProductionCompanies;
                worksheet.Cell($"V{rowCounter}").Value = egedaRow.Percentage;
                worksheet.Cell($"W{rowCounter}").Value = egedaRow.RightsFrom;
                worksheet.Cell($"X{rowCounter}").Value = egedaRow.RightsTo;

                rowCounter++;
            }

            using var ms = new MemoryStream();
            package.SaveAs(ms);
            ms.Position = 0;

            if (blobClient.Exists())
                blobClient.Delete();

            blobClient.Upload(ms);

            return Result.Ok();
        }

        public Result ExportRegistrations(RegistrationWorksGWFFDto registrationWorksGwff)
        {
            var resource = typeof(ExportService).Assembly.GetManifestResourceStream("Oscar.Infrastructure.Features.Registration.Templates.GWFFTemplate.xlsx");

            var containerClient = _blobServiceClient.GetBlobContainerClient("oscar-registrations");
            var blobClient = containerClient.GetBlobClient(registrationWorksGwff.FileName);

            using var package = new XLWorkbook(resource);
            var worksheet = package.Worksheet(1);
            worksheet.ShowGridLines = true;


            worksheet.Cell($"A1").Value = $"COMPACT COLLECTIONS LTD - WORKS PRODUCTION YEAR UP TO AND INCLUDING {DateTime.Now.Year}";

            var rowCounter = 4;
            foreach (var gwffRow in registrationWorksGwff.Rows)
            {
                if (rowCounter > 4)
                    CopyRow(worksheet, 4, rowCounter);

                worksheet.Cell($"A{rowCounter}").SetValue(gwffRow.ClientName);
                worksheet.Cell($"B{rowCounter}").Value = gwffRow.TitleOfSeries;
                worksheet.Cell($"C{rowCounter}").Value = gwffRow.OriginalTitle;
                worksheet.Cell($"D{rowCounter}").Value = gwffRow.GermanTitle;
                worksheet.Cell($"E{rowCounter}").Value = gwffRow.TitleOfEpisodes;
                worksheet.Cell($"F{rowCounter}").Value = gwffRow.YearOfProduction;
                worksheet.Cell($"G{rowCounter}").Value = gwffRow.Duration;
                worksheet.Cell($"H{rowCounter}").Value = gwffRow.TypeOfWork;
                worksheet.Cell($"I{rowCounter}").Value = gwffRow.Genre;
                worksheet.Cell($"J{rowCounter}").Value = gwffRow.ProductionCompanies;
                worksheet.Cell($"K{rowCounter}").Value = gwffRow.Directors;
                worksheet.Cell($"L{rowCounter}").Value = gwffRow.Actors;
                worksheet.Cell($"M{rowCounter}").Value = gwffRow.ProductionCountries;
                worksheet.Cell($"N{rowCounter}").Value = gwffRow.Percentage;
                worksheet.Cell($"O{rowCounter}").Value = gwffRow.PeriodFrom;
                worksheet.Cell($"P{rowCounter}").Value = gwffRow.PeriodTo;
                worksheet.Cell($"Q{rowCounter}").Value = gwffRow.SeasonCount;
                worksheet.Cell($"R{rowCounter}").Value = gwffRow.EpisodeCount;
                worksheet.Cell($"S{rowCounter}").DataType = XLDataType.Text;
                worksheet.Cell($"T{rowCounter}").DataType = XLDataType.Text;
                worksheet.Cell($"S{rowCounter}").SetValue(gwffRow.SeriesCompactNo);
                worksheet.Cell($"T{rowCounter}").SetValue(gwffRow.CompactNo);
                worksheet.Cell($"U{rowCounter}").Value = gwffRow.GWFFNo;
                worksheet.Cell($"V{rowCounter}").Value = gwffRow.AgicoaNo;
                worksheet.Cell($"W{rowCounter}").Value = gwffRow.IsanNo;
                worksheet.Cell($"X{rowCounter}").Value = gwffRow.VamNo;
                worksheet.Cell($"Y{rowCounter}").Value = gwffRow.SuissImageNo;

                rowCounter++;
            }

            using var ms = new MemoryStream();
            package.SaveAs(ms);
            ms.Position = 0;

            if (blobClient.Exists())
                blobClient.Delete();

            blobClient.Upload(ms);

            return Result.Ok();
        }

        public Result ExportRegistrations(RegistrationWorksMPADto registrationWorksMpa)
        {
            var resource = typeof(ExportService).Assembly
                .GetManifestResourceStream("Oscar.Infrastructure.Features.Registration.Templates.MPATemplate.xlsx");

            var containerClient = _blobServiceClient.GetBlobContainerClient("oscar-registrations");
            BlobClient blobClient = containerClient.GetBlobClient(registrationWorksMpa.FileName);

            using var package = new XLWorkbook(resource);
            var claimantInformation = package.Worksheet(1);
            var claimedWorks = package.Worksheet(2);
            claimedWorks.ShowGridLines = false;
            claimedWorks.Name = $"{DateTime.Now.Year} Claimed Works";

            claimantInformation.Cell($"A1").Value =
                $"MOTION PICTURE ASSOCIATION, INC. CLAIMANT INFORMATION AS OF {DateTime.Now.ToLongDateString()}";

            claimantInformation.Cell($"A6").Value = $"MPA VENDOR ID {registrationWorksMpa.VendorId}: {registrationWorksMpa.ClientName}";

            claimedWorks.Cell($"A3").Value = $"MOTION PICTURE ASSOCIATION, INC. CLAIMED WORKS {DateTime.Now.Year}";
            claimedWorks.Cell($"A5").Value = $"MPA VENDOR ID: {registrationWorksMpa.ClientName}";

            var rowCounter = 10;
            foreach (var mpaRow in registrationWorksMpa.Rows)
            {
                if (rowCounter > 10)
                    CopyRow(claimedWorks, 10, rowCounter);

                claimedWorks.Cell($"A{rowCounter}").SetValue(mpaRow.ClaimantId);
                claimedWorks.Cell($"B{rowCounter}").SetValue(mpaRow.ReferenceId);
                claimedWorks.Cell($"C{rowCounter}").SetValue(mpaRow.CableNetwork);
                claimedWorks.Cell($"D{rowCounter}").Value = mpaRow.CableSyndicated;
                claimedWorks.Cell($"E{rowCounter}").Value = mpaRow.SatelliteNetwork;
                claimedWorks.Cell($"F{rowCounter}").Value = mpaRow.SatelliteSyndicated;
                claimedWorks.Cell($"G{rowCounter}").Value = mpaRow.Title;
                claimedWorks.Cell($"H{rowCounter}").Value = mpaRow.EpisodeTitle;
                claimedWorks.Cell($"I{rowCounter}").Value = mpaRow.Genre;
                claimedWorks.Cell($"J{rowCounter}").Value = mpaRow.ProductionYear;
                claimedWorks.Cell($"K{rowCounter}").Value = mpaRow.CountryIfNotUS;
                claimedWorks.Cell($"L{rowCounter}").Value = mpaRow.DurationMinutes;
                claimedWorks.Cell($"M{rowCounter}").Value = mpaRow.ClaimStartDate;
                claimedWorks.Cell($"N{rowCounter}").Value = mpaRow.ClaimEndDate;
                claimedWorks.Cell($"O{rowCounter}").Value = mpaRow.Cast;
                claimedWorks.Cell($"P{rowCounter}").Value = mpaRow.Isan;
                claimedWorks.Cell($"Q{rowCounter}").Value = mpaRow.Eidr;

                rowCounter++;
            }

            using var ms = new MemoryStream();
            package.SaveAs(ms);
            ms.Position = 0;

            if (blobClient.Exists())
                blobClient.Delete();

            blobClient.Upload(ms);

            return Result.Ok();
        }

        public Result ExportRegistrations(RegistrationWorksUpfarArgoaDto registrationWorksMpa)
        {
            var resource = typeof(ExportService).Assembly.GetManifestResourceStream("Oscar.Infrastructure.Features.Registration.Templates.UpfarArgoaTemplate.xlsx");

            var containerClient = _blobServiceClient.GetBlobContainerClient("oscar-registrations");
            var blobClient = containerClient.GetBlobClient(registrationWorksMpa.FileName);

            using var package = new XLWorkbook(resource);
            var worksheet = package.Worksheet(1);
            worksheet.ShowGridLines = true;

            var rowCounter = 2;
            var no = 1;
            foreach (var upfarArgoaRow in registrationWorksMpa.Rows)
            {
                if (rowCounter > 2)
                    CopyRow(worksheet, 2, rowCounter);

                worksheet.Cell($"A{rowCounter}").SetValue(no);
                worksheet.Cell($"B{rowCounter}").Value = upfarArgoaRow.SeriesOrStandAloneTitle;
                worksheet.Cell($"C{rowCounter}").Value = upfarArgoaRow.EpisodeTitle;
                worksheet.Cell($"D{rowCounter}").Value = upfarArgoaRow.SeasonTitle;
                worksheet.Cell($"E{rowCounter}").Value = upfarArgoaRow.WorkType;
                worksheet.Cell($"F{rowCounter}").Value = upfarArgoaRow.ProductionCountry;
                worksheet.Cell($"G{rowCounter}").Value = upfarArgoaRow.IdentificationCode;
                worksheet.Cell($"H{rowCounter}").Value = upfarArgoaRow.RightHolder;
                worksheet.Cell($"I{rowCounter}").Value = upfarArgoaRow.Producer;
                worksheet.Cell($"J{rowCounter}").Value = upfarArgoaRow.Performer;

                //worksheet.Cell($"K{rowCounter}").Value = upfarArgoaRow.Author;
                //worksheet.Cell($"L{rowCounter}").Value = upfarArgoaRow.RightsSpecifics;
                worksheet.Cell($"M{rowCounter}").Value = upfarArgoaRow.ManagedRightsRetransmission;
                worksheet.Cell($"N{rowCounter}").Value = upfarArgoaRow.ManagedRightsPrivate;
                worksheet.Cell($"O{rowCounter}").Value = upfarArgoaRow.ManagedRightsPublic;

                // worksheet.Cell($"J{rowCounter}").DataType = XLDataType.Text;
                //worksheet.Cell($"J{rowCounter}").SetValue(upfarArgoaRow.ManagedRights);

                worksheet.Cell($"P{rowCounter}").DataType = XLDataType.Text;
                worksheet.Cell($"P{rowCounter}").SetValue(upfarArgoaRow.QuotaRightsHeld);

                worksheet.Cell($"Q{rowCounter}").DataType = XLDataType.Text;
                worksheet.Cell($"Q{rowCounter}").SetValue(upfarArgoaRow.Duration);

                worksheet.Cell($"R{rowCounter}").Style.NumberFormat.Format = "yyyy-MM-dd";
                worksheet.Cell($"R{rowCounter}").Value = upfarArgoaRow.DateOfRegistration;

                worksheet.Cell($"S{rowCounter}").Value = upfarArgoaRow.YearOfCalculating;
                worksheet.Cell($"T{rowCounter}").Value = upfarArgoaRow.ReciprocalContracts;

                worksheet.Cell($"X{rowCounter}").Value = "Yes";
                worksheet.Cell($"Y{rowCounter}").Value = "No";

                //worksheet.Cell($"P{rowCounter}").Value = upfarArgoaRow.Observations;
                worksheet.Cell($"AD{rowCounter}").Style.NumberFormat.Format = "yyyy-MM-dd";
                worksheet.Cell($"AD{rowCounter}").Value = upfarArgoaRow.RightsFrom;
                worksheet.Cell($"AE{rowCounter}").Style.NumberFormat.Format = "yyyy-MM-dd";
                worksheet.Cell($"AE{rowCounter}").Value = upfarArgoaRow.RightsTo;

                rowCounter++;
                no++;
            }

            using var ms = new MemoryStream();  
            package.SaveAs(ms);
            ms.Position = 0;

            if (blobClient.Exists())
                blobClient.Delete();

            blobClient.Upload(ms);

            return Result.Ok();
        }

        public Result<string> ExportEquivalenceListAsCsv(string filename, List<EquivalenceDto> equivalenceList)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(ContainerName.EQUIVALENCE);
                var blobClient = containerClient.GetBlobClient(filename);

                var myConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ","
                };

                using (var memoryStream = new MemoryStream())
                {
                    using (var writer = new StreamWriter(memoryStream))
                    using (var csv = new CsvWriter(writer, myConfig))
                    {
                        csv.Context.RegisterClassMap<EquivalenceDtoMap>();
                        csv.WriteRecords(equivalenceList);
                        writer.Flush();
                        memoryStream.Position = 0;  // Reset stream position for reading

                        blobClient.Upload(memoryStream, new BlobHttpHeaders { ContentType = "text/csv" });
                    }
                }

                var fileUrl = blobClient.Uri.ToString();
                return Result.Ok(fileUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError((int)ImportExportServiceEvent.ExportError, ex.Message);
                return Result.Fail<string>(ex.Message);
            }
        }

        private void CopyRow(IXLWorksheet detailsSheet, int from, int to)
        {
            if (from != to)
            {
                detailsSheet.Range(from, 1, from, 18).CopyTo(detailsSheet.Range(to, 1, to, 18));
                for (int col = 1; col <= 18; col++)
                {
                    var toCell = detailsSheet.Cell(to, col);
                    toCell.SetValue(string.Empty);
                }
            }
        }

        public static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }


    }
}
