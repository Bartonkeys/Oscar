using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Core.Entities;
using Oscar.Data.Context;
using System.Linq.Expressions;
using Oscar.Infrastructure.Features.Common;
using Oscar.Infrastructure.Features.Report.Services;
using System.Dynamic;
using System.Data.SqlClient;
using System.Data;
using Newtonsoft.Json;

namespace Oscar.Infrastructure.Features.Report.Queries
{
    public class GetReportDataByIdQuery : BaseTableQuery, IRequest<Result<ReportDataDto>>
    {
        public int Id { get; set; }

        public GetReportDataByIdQuery()
        {
        
        }

    }

    public class GetReportDataByIdHandler : AbstractBaseHandler<GetReportDataByIdQuery, ReportDataDto>
    {
        public GetReportDataByIdHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetReportDataByIdQuery> validator, ILogger<GetReportDataByIdQuery> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<ReportDataDto>> HandleRequest(GetReportDataByIdQuery request, CancellationToken cancellationToken)
        {
            
            var report = await OscarContext.Reports
                            .Include(i => i.ReportFields)
                            .SingleOrDefaultAsync(w => w.Id == request.Id);
            report.ReportStatus = ReportStatus.Building;
            await OscarContext.SaveChangesAsync(cancellationToken);

            var reportDto = Mapper.Map<ReportDto>(report);

            if (reportDto == null)
            {
                Logger.LogError((int)ReportFeatureEvent.Get, CommandResult.ERROR + " : " + "Report not found");
                return Result.Fail<ReportDataDto>(CommandResult.ERROR + " : " + "Report not found");
            }

            var queryString = ReportHelperService.BuildQueryFromReportFieldsAndSearchObjects(reportDto, request.SearchObjects, OscarContext);

            DataTable testTable = new DataTable();
            var reportDataDto = new ReportDataDto();

            try
            {
                using (var command = OscarContext.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = queryString;
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = 60 * 60;

                    OscarContext.Database.OpenConnection();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var table = new DataTable();
                        testTable.BeginLoadData();
                        testTable.Load(reader);
                        testTable.EndLoadData();
                    }
                }

                foreach (DataRow row in testTable.Rows)
                {
                    if (row.ItemArray != null && row.ItemArray[0] !=  null)
                    {
                        var jsonString = row.ItemArray[0].ToString();
                        dynamic unescapedJson = JsonConvert.DeserializeObject(jsonString);
                        reportDataDto.AddReportDataItem(unescapedJson);
                    }
                }
            }
            catch(Exception ex)
            {
                Logger.LogError((int)ReportFeatureEvent.Get, CommandResult.ERROR + " : " + ex.Message);
                return Result.Fail<ReportDataDto>(CommandResult.ERROR);
            }
            finally
            {
                OscarContext.Database.CloseConnection();
            }

            Logger.LogInformation((int)ReportFeatureEvent.Get, CommandResult.SUCCESS);
           
            return Result.Ok(reportDataDto);
        }

    }
}
