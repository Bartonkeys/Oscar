using BartonKeys.Functional;
using Microsoft.AspNetCore.Http;

namespace Oscar.Infrastructure.Features.Common.Contracts
{
    public interface IContainerService
    {
        Task<Result<string>> UploadAsync(IFormFile file, string containerName, int requestId, string? fileExtension, string? folderName, CancellationToken cancellationToken);
        Task<Result<string>> DeleteBlob(string containerName, string blobName, CancellationToken cancellationToken);

    }
}
