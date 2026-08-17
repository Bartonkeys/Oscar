using BartonKeys.Functional;
using Microsoft.AspNetCore.Http;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;

namespace Oscar.Infrastructure.Features.WorksImport.Services
{
    public interface IWorksImportService
    {
        List<WorksImportDto> WorksImportDtoListFromFile(IFormFile? formFile, bool isAgicoa);
        List<EpisodeImportDto> EpisodeImportDtoListFromFile(IFormFile? formFile);
        Task<Result> WriteWorksRecords(WorksImportRequest worksImportRequest, Client client, Oscar.Core.Entities.Catalogue? catalogue);
        Task CheckForDuplicates(ICollection< Core.Entities.WorksImport> worksImportList, CancellationToken cancellationToken);
    }
}