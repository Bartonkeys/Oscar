using Microsoft.AspNetCore.Mvc;
using Oscar.Blazor.Library.Services;

namespace Oscar.Blazor.Library.Controllers;

public class ExportOscarDataController : ExportController
{
    private readonly OscarDataService _service;

    public ExportOscarDataController(OscarDataService service)
    {
        _service = service;
    }

    [HttpGet("/export/OscarData/countries/csv")]
    [HttpGet("/export/OscarData/countries/csv(fileName='{fileName}')")]
    public async Task<FileStreamResult> ExportCountriesToCSV(string fileName = null)
    {
        return ToCSV(ApplyQuery(await _service.GetCountries(), Request.Query, true), fileName);
    }

    [HttpGet("/export/OscarData/countries/excel")]
    [HttpGet("/export/OscarData/countries/excel(fileName='{fileName}')")]
    public async Task<FileStreamResult> ExportCountriesToExcel(string fileName = null)
    {
        return ToExcel(ApplyQuery(await _service.GetCountries(), Request.Query, true), fileName);
    }

    [HttpGet("/export/OscarData/clientdetails/csv")]
    [HttpGet("/export/OscarData/clientdetails/csv(fileName='{fileName}')")]
    public async Task<FileStreamResult> ExportClientDetailsToCSV(string fileName = null)
    {
        return ToCSV(ApplyQuery(await _service.GetClientDetails(), Request.Query, true), fileName);
    }

    [HttpGet("/export/OscarData/clientdetails/excel")]
    [HttpGet("/export/OscarData/clientdetails/excel(fileName='{fileName}')")]
    public async Task<FileStreamResult> ExportClientDetailsToExcel(string fileName = null)
    {
        return ToExcel(ApplyQuery(await _service.GetClientDetails(), Request.Query, true), fileName);
    }

    [HttpGet("/export/OscarData/clientcataloguesdetails/csv")]
    [HttpGet("/export/OscarData/clientcataloguesdetails/csv(fileName='{fileName}')")]
    public async Task<FileStreamResult> ExportClientCataloguesDetailsToCSV(string fileName = null)
    {
        return ToCSV(ApplyQuery(await _service.GetClientCataloguesDetails(), Request.Query, true), fileName);
    }

    [HttpGet("/export/OscarData/clientcataloguesdetails/excel")]
    [HttpGet("/export/OscarData/clientcataloguesdetails/excel(fileName='{fileName}')")]
    public async Task<FileStreamResult> ExportClientCataloguesDetailsToExcel(string fileName = null)
    {
        return ToExcel(ApplyQuery(await _service.GetClientCataloguesDetails(), Request.Query, true), fileName);
    }

    [HttpGet("/export/OscarData/clientworkslist/csv")]
    [HttpGet("/export/OscarData/clientworkslist/csv(fileName='{fileName}')")]
    public async Task<FileStreamResult> ExportClientWorksListToCSV(string fileName = null)
    {
        return ToCSV(ApplyQuery(await _service.GetClientWorksList(), Request.Query, true), fileName);
    }

    [HttpGet("/export/OscarData/clientworkslist/excel")]
    [HttpGet("/export/OscarData/clientworkslist/excel(fileName='{fileName}')")]
    public async Task<FileStreamResult> ExportClientWorksListToExcel(string fileName = null)
    {
        return ToExcel(ApplyQuery(await _service.GetClientWorksList(), Request.Query, true), fileName);
    }

    [HttpGet("/export/OscarData/productioncountryitems/csv")]
    [HttpGet("/export/OscarData/productioncountryitems/csv(fileName='{fileName}')")]
    public async Task<FileStreamResult> ExportProductionCountryItemsToCSV(string fileName = null)
    {
        return ToCSV(ApplyQuery(await _service.GetProductionCountriesItems(), Request.Query, true), fileName);
    }

    [HttpGet("/export/OscarData/productioncountryitems/excel")]
    [HttpGet("/export/OscarData/productioncountryitems/excel(fileName='{fileName}')")]
    public async Task<FileStreamResult> ExportProductionCountryItemsToExcel(string fileName = null)
    {
        return ToExcel(ApplyQuery(await _service.GetProductionCountriesItems(), Request.Query, true), fileName);
    }

    [HttpGet("/export/OscarData/clientworkrights/csv")]
    [HttpGet("/export/OscarData/clientworkrights/csv(fileName='{fileName}')")]
    public async Task<FileStreamResult> ExportClientWorkRightsToCSV(string fileName = null)
    {
        return ToCSV(ApplyQuery(await _service.GetClientWorkRightItems(), Request.Query, true), fileName);
    }

    [HttpGet("/export/OscarData/clientworkrights/excel")]
    [HttpGet("/export/OscarData/clientworkrights/excel(fileName='{fileName}')")]
    public async Task<FileStreamResult> ExportClientWorkRightsToExcel(string fileName = null)
    {
        return ToExcel(ApplyQuery(await _service.GetClientWorkRightItems(), Request.Query, true), fileName);
    }

    //
    [HttpGet("/export/OscarData/clientyearlystats/csv")]
    [HttpGet("/export/OscarData/clientyearlystats/csv(fileName='{fileName}')")]
    public async Task<FileStreamResult> ExportClientYearlyStatsToCSV(string fileName = null)
    {
        return ToCSV(ApplyQuery(await _service.GetClientYearlyStats(), Request.Query, true), fileName);
    }

    [HttpGet("/export/OscarData/clientyearlystats/excel")]
    [HttpGet("/export/OscarData/clientyearlystats/excel(fileName='{fileName}')")]
    public async Task<FileStreamResult> ExportClientYearlyStatsToExcel(string fileName = null)
    {
        return ToExcel(ApplyQuery(await _service.GetClientYearlyStats(), Request.Query, true), fileName);
    }
}