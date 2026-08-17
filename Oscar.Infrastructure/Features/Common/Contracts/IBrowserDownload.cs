using System;
using BartonKeys.Functional;
using Oscar.Core.DTOs;

namespace Oscar.Infrastructure.Features.Common.Contracts
{
    public interface IBrowserDownload
    {
        Task<Result> ExportWorksAsCsv(IEnumerable<WorksDto> worksDtos, string fileName);
    }
}

