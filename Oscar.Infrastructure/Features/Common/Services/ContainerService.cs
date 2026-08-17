using Azure.Storage.Blobs;
using BartonKeys.Functional;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Oscar.Core.Enums;
using Oscar.Infrastructure.Features.Common.Contracts;

namespace Oscar.Infrastructure.Features.Common.Services
{


    public class ContainerService : IContainerService
    {

        private BlobServiceClient _blobServiceClient;
        private ILogger<ContainerService> _logger;

        public ContainerService(BlobServiceClient blobServiceClient, ILogger<ContainerService> logger)
        {
            _blobServiceClient =  blobServiceClient;
            _logger = logger;
        }

        public async Task<Result<string>> UploadAsync(IFormFile file, string containerName,  int requestId, string? fileExtension, string? folderName, CancellationToken cancellationToken)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var ms = new MemoryStream();
            file?.CopyTo(ms);
            ms.Position = 0;
            var reference = String.Format("{0:yyyy_MM_dd}_{1}", DateTime.Now, requestId);
            if (fileExtension != null) { reference += $"{fileExtension}"; }
            var blobName = $"{reference}";
            if (folderName != null) { blobName = folderName + Path.DirectorySeparatorChar + blobName; }
            var uploadResult = await containerClient.UploadBlobAsync(blobName, ms, cancellationToken);

            var blobResponse = uploadResult.GetRawResponse();
            if (blobResponse.IsError)
            {
                _logger.LogError((int)AzureStorage.BlobUpload, blobResponse.ReasonPhrase);
                return Result.Fail<string>(blobResponse.ReasonPhrase);
            }
            return Result.Ok(reference);
        }

        public async Task<Result<string>> DeleteBlob(string containerName, string blobName, CancellationToken cancellationToken)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            
            var deleteResult = await containerClient.DeleteBlobIfExistsAsync(blobName);
            if (!deleteResult)
            {
                _logger.LogError((int)AzureStorage.BlobDelete, containerName + blobName);
                return Result.Fail<string>(containerName + blobName);
            }
            return Result.Ok("Successfully deleted blob");

        }

    }
}
