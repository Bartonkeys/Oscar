using System.IO.Compression;
using System.Xml.Linq;
using AutoMapper;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using Oscar.Core.Entities;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;

namespace Oscar.Infrastructure.Features.Registration.Commands
{
    public class StitchOrZipRegistrationsCommand : IRequest<Result>
    {
        public List<string> FileResults { get; set; }
        public Guid BatchId { get; set; }
    }

    public class StitchOrZipRegistrationsCommandHandler : SimpleAbstractBaseHandler<StitchOrZipRegistrationsCommand>
    {
        private const string BlobContainerName = "oscar-registrations";
        private readonly BlobServiceClient _blobServiceClient;

        public StitchOrZipRegistrationsCommandHandler(OscarContext oscarContext, IMapper mapper,
            IValidator<StitchOrZipRegistrationsCommand> validator, ILogger<StitchOrZipRegistrationsCommand> logger,
            BlobServiceClient blobServiceClient) : base(oscarContext, mapper, validator, logger)
        {
            _blobServiceClient = blobServiceClient;
        }

        protected override async Task<Result> HandleRequest(StitchOrZipRegistrationsCommand request,
            CancellationToken cancellationToken)
        {
            var registrationBatch =
                await OscarContext.RegistrationBatches.SingleOrDefaultAsync(r => r.BatchId == request.BatchId,
                    cancellationToken: cancellationToken);

            var societyName = (await OscarContext.Societies.SingleAsync(s => s.Id == registrationBatch!.SocietyId, cancellationToken: cancellationToken)).Name.ToUpper();

            if (registrationBatch == null || !registrationBatch!.IsAllClients) 
                return Result.Ok();

            string blobName;

            if (societyName == "UPFAR ARGOA" || societyName == "CMC" || societyName == "MPLC")
            {
                blobName = $"{request.BatchId}.xlsx";

                if (CheckAllProcessedFilesExistsInBlobAsync(request.FileResults, blobName, registrationBatch, cancellationToken).Result)
                    await StitchAndUploadExcelFilesAsync(request.FileResults, blobName, societyName);
            }
            else if (societyName == "SUISSIMAGE")
            {
                blobName = $"{request.BatchId}.zip";

                if (CheckAllProcessedFilesExistsInBlobAsync(request.FileResults, blobName, registrationBatch, cancellationToken).Result)
                    await StitchAndUploadSuissImageXmlFilesAsync(request.FileResults, blobName);
            }
            else
            {
                blobName = $"{request.BatchId}.zip";

                if (CheckAllProcessedFilesExistsInBlobAsync(request.FileResults, blobName, registrationBatch, cancellationToken).Result)
                    await ZipAndUploadFilesAsync(request.FileResults, blobName);
            }

            registrationBatch.FileName = blobName;
            registrationBatch.RegisterStatus = RegisterStatus.Batch_Export_Success;
            await OscarContext.SaveChangesAsync(cancellationToken);
            return Result.Ok();
        }

        private async Task<bool> ZipAndUploadFilesAsync(List<string> fileUrls, string blobName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(BlobContainerName);

            var blockBlobClient = containerClient.GetBlockBlobClient(blobName);

            await using var targetBlobStream = await blockBlobClient.OpenWriteAsync(true);

            using var zipArchive = new ZipArchive(targetBlobStream, ZipArchiveMode.Create);
            foreach (var fileUrl in fileUrls)
            {
                var blobClient = containerClient.GetBlockBlobClient(fileUrl);

                var zipArchiveEntry = zipArchive.CreateEntry(fileUrl);
                await using var fileInZipStream = zipArchiveEntry.Open();
                {
                    await using var readBlobStream =
                        await blobClient.OpenReadAsync();
                    {
                        await readBlobStream.CopyToAsync(fileInZipStream);
                    }
                }
            }

            return true;
        }

        private async Task<bool> StitchAndUploadExcelFilesAsync(List<string> fileUrls, string blobName, string societyName)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            string stitchedFileName = blobName;

            var containerClient = _blobServiceClient.GetBlobContainerClient(BlobContainerName);
            var outputBlobClient = containerClient.GetBlockBlobClient(stitchedFileName);

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Sheet1");

            int currentRow = 1;
            int sequenceNo = 0;
            bool isFirstFile = true;

            foreach (var fileUrl in fileUrls)
            {
                var blobClient = containerClient.GetBlobClient(fileUrl);
                await using var ms = new MemoryStream();
                await blobClient.DownloadToAsync(ms);
                ms.Position = 0;

                using var tempPackage = new ExcelPackage(ms);
                var tempWorksheet = tempPackage.Workbook.Worksheets[0];

                if (isFirstFile)
                {
                    for (int col = 1; col <= tempWorksheet.Dimension.End.Column; col++)
                    {
                        worksheet.Column(col).Width = tempWorksheet.Column(col).Width;
                    }
                }

                for (int rowNum = 1; rowNum <= tempWorksheet.Dimension.End.Row; rowNum++)
                {
                    worksheet.Row(currentRow).Height = tempWorksheet.Row(rowNum).Height;

                    //If the current row has a value, copy the row
                    if (tempWorksheet.Cells[rowNum, 1, rowNum, tempWorksheet.Dimension.End.Column].Any(cell => cell.Value != null))
                    {
                        if (SkipRow(isFirstFile, societyName, rowNum))
                            continue;

                        for (int colNum = 1; colNum <= tempWorksheet.Dimension.End.Column; colNum++)
                        {
                            // Copy the cell value and formatting from the temp worksheet to the main worksheet
                            var sourceCell = tempWorksheet.Cells[rowNum, colNum];
                            var targetCell = worksheet.Cells[currentRow, colNum];

                            if (societyName == "UPFAR ARGOA" && rowNum > 1 && colNum == 1)
                                targetCell.Value = sequenceNo;
                            else
                                targetCell.Value = sourceCell.Value;

                            sourceCell.CopyStyles(targetCell);
                        }
                        currentRow++;
                        sequenceNo++;
                    }
                }

                isFirstFile = false;
            }

            // Save the package to a memory stream
            await using var outputStream = new MemoryStream();
            package.SaveAs(outputStream);
            outputStream.Position = 0; // Reset the position to the beginning for uploading

            // Upload the merged Excel file to the blob storage
            await outputBlobClient.UploadAsync(outputStream);

            return true;
        }

        //Different society may need to skip different rows when stitching 2 or more files into 1 file
        private bool SkipRow(bool isFirstFile, string societyName, int rowNum)
        {
            if (!isFirstFile)
            {
                if (societyName == "UPFAR ARGOA" || societyName == "CMC" || societyName == "MPLC")
                {
                    if (rowNum == 1)
                        return true;
                }
            }
            
            return false;
        }

        private async Task<bool> StitchAndUploadSuissImageXmlFilesAsync(List<string> fileUrls, string blobName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(BlobContainerName);
            var outputBlobClient = containerClient.GetBlockBlobClient(blobName);

            XDocument combinedDocument = new XDocument(new XElement("Data"));
            XElement headerElement = null;
            XElement footerElement = null;

            foreach (var fileUrl in fileUrls)
            {
                var blobClient = containerClient.GetBlobClient(fileUrl);
                await using var ms = new MemoryStream();
                await blobClient.DownloadToAsync(ms);
                ms.Position = 0;

                XDocument currentDocument = XDocument.Load(ms);

                // Add Header and Footer elements only once
                if (headerElement == null)
                {
                    headerElement = currentDocument.Root.Element("Header");
                    combinedDocument.Root.Add(headerElement);
                }

                if (footerElement == null)
                {
                    footerElement = currentDocument.Root.Element("Footer");
                }

                // Add Work elements
                combinedDocument.Root.Add(currentDocument.Root.Elements("Work"));
            }

            combinedDocument.Root.Add(footerElement);

            await using var zipMemoryStream = new MemoryStream();
            using (var archive = new ZipArchive(zipMemoryStream, ZipArchiveMode.Create, true))
            {
                // Add the XML document to the zip archive
                var entry = archive.CreateEntry("AllClients.xml");
                using (var entryStream = entry.Open())
                using (var writer = new StreamWriter(entryStream))
                {
                    combinedDocument.Save(writer);
                }
            }

            // Reset the position of the zipMemoryStream to start
            zipMemoryStream.Position = 0;

            // Upload the zipped content to the blob storage
            await outputBlobClient.UploadAsync(zipMemoryStream);
            
            return true;
        }

        //Blob uploader may take some time to upload processed files
        //so, check if the files are uploaded and available in blob store else give some time
        //and retry and check again after some time
        private async Task<bool> CheckAllProcessedFilesExistsInBlobAsync(List<string> fileUrls, string blobName, RegistrationBatch registrationBatch, CancellationToken cancellationToken)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(BlobContainerName);

            // Check if all files exist in the blob storage container
            foreach (var fileUrl in fileUrls)
            {
                var blobClient = containerClient.GetBlobClient(fileUrl);

                // Poll until the file exists or timeout occurs
                bool fileExists = false;
                int retries = 0;
                int maxRetries = 10;
                TimeSpan delayBetweenRetries = TimeSpan.FromSeconds(60);

                while (!fileExists && retries < maxRetries)
                {
                    fileExists = await blobClient.ExistsAsync();
                    if (!fileExists)
                    {
                        retries++;
                        await Task.Delay(delayBetweenRetries);
                    }
                }

                if (!fileExists)
                {
                    registrationBatch.FileName = blobName;
                    registrationBatch.RegisterStatus = RegisterStatus.Failed;
                    registrationBatch.Notes= "One or more processed clients file don't exist in blob store";
                    await OscarContext.SaveChangesAsync(cancellationToken);
                    return false;
                }
            }
            return true;
        }
        }
}

