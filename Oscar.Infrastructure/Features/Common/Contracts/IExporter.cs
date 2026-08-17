using BartonKeys.Functional;
using Oscar.Core.DTOs;
using Oscar.Core.Schemas;

namespace Oscar.Infrastructure.Features.Common.Contracts;

public interface IExporter
{
    Result ExportListAsCsv(IEnumerable<MatchTemplateResultsDto> matchTemplateDtos, string fileName);
    Result ExportWorksImportAsCsv(IEnumerable<WorksImportDto> worksImportDtos, string fileName);
    Result ExportRegistrationsAsCsv(IEnumerable<RegistrationCreateDto> registrationCreateDtos, string fileName);
    Result ExportRegistrationsAsXml(IEnumerable<RegistrationCreateDto> registrationCreateDtos, string fileName);
    Result ExportRegistrations(IRegistration agicoaWri);
    Result ExportRegistrations(IRegistrationWorksScreenrights screenrightsWri);
    Result ExportRegistrations(RegistrationWorksCCCDto registrationWorksCcc);
    Result ExportRegistrations(RegistrationWorksCMCDto registrationWorksCcmc);
    Result ExportRegistrations(RegistrationWorksMPLCDto registrationWorksMplc);
    Result ExportRegistrations(RegistrationWorksCRCDto registrationWorksCrc);
    Result ExportRegistrations(RegistrationWorksEGEDADto registrationWorksEgeda);
    Result ExportRegistrations(RegistrationWorksGWFFDto registrationWorksGwff);
    Result ExportRegistrations(RegistrationWorksMPADto registrationWorksMpa);
    Result ExportRegistrations(RegistrationWorksUpfarArgoaDto registrationWorksMpa);
    Task<Result<string>> ExportReportsAsCsv(ReportDataDto reportDataDto, string fileName);
    Result<string> ExportEquivalenceListAsCsv(string filename, List<EquivalenceDto> equivalenceList);
}