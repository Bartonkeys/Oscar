using BartonKeys.Functional;
using Oscar.Core.DTOs;

namespace Oscar.Infrastructure.Features.Common.Contracts;

public interface IImporter
{
    Result<List<MatchTemplateDto>> ImportMatchCsvAsList(string filename);
    Result<byte[]> ImportMatchBlobAsBytes(string filename);

    Result<List<WorksImportDto>> ImportWorksCsvAsList(string filename);

    Result<List<EquivalenceDto>> ImportEquivalenceCsvAsList(string filename);

    Result<List<ScreenrightsDto>> ImportScreenrightsCsvAsList(string filename);



}