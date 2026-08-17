using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Infrastructure.Features.Report.Commands;
using Oscar.Infrastructure.Features.Report.Queries;
using Oscar.Infrastructure.Features.Report.Services;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Oscar.Integration.Tests.Report
{
    [TestClass]
    public class ReportFeatureShouldBeAbleTo : BaseTest
    {


        [TestMethod]
        public async Task AddReport()
        {
            // Arrange

            var reportAddDto = new ReportDto
            {
            
                ReportName = "Clients_with_enums",
                BaseEntityName =  "Clients",
                ReportFields  = new ReportFieldDto[]
                {
                    new ReportFieldDto{ BaseEntityName = "Clients", ReportFieldName = "Id" },
                    new ReportFieldDto{ BaseEntityName = "Clients", ReportFieldName = "ClientName" },
                    new ReportFieldDto{ BaseEntityName = "Works", ReportFieldName = "Id" },
                    new ReportFieldDto{ BaseEntityName = "Works", ReportFieldName = "WorksStatus" }
                }

            };

            var addReportCommand = new AddReportCommand
            {
                ReportAddDto = reportAddDto
            };

            // Act
            var result = await Mediator.Send(addReportCommand);
            var reportDto = OscarContext.Reports.OrderByDescending(x => x.Id).First();

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Value.Id > 0);
            Assert.AreEqual(result.Value?.ReportFields?.Count, reportAddDto.ReportFields.Count);

        }


        [TestMethod]
        public async Task GetReportById()
        {
            // Arrange
            var report = OscarContext.Reports.First();

            // Act
            var result = await Mediator.Send(new GetReportByIdQuery { Id = report.Id });

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(report.Id, result.Value.Id);
        }

        [TestMethod]
        public async Task GetReportByName()
        {
            // Arrange
            var report = OscarContext.Reports.Last();

            // Act
            var request = new GetReportsQuery();
            var searchObject = new SearchObject("Report", "string", "ReportName", report.ReportName);
            request.SearchObjects.Add(searchObject);
            var result = await Mediator.Send(request);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(report.Id, result.Value.Records.First().Id);
            Assert.IsTrue(result.Value.Records.ToList().Count() == 1);
            Assert.IsTrue(result.Value.TotalRecords == 1);
        }

        //TODO: Cannot use standard in memory DB as the GetReportDataByIdQuery code relies on pure SQL calls
        //[TestMethod]
        //public async Task GetReportData()
        //{
            //// Arrange
            //var report = OscarContext.Reports.First();
            //var client = OscarContext.Clients.First();

            //// Act
            //var request = new GetReportDataByIdQuery();
            //var searchObject = new SearchObject("Clients", "number", "Id", client.Id.ToString());
            //request.SearchObjects.Add(searchObject);
            //request.Id = report.Id;
            //var result = await Mediator.Send(request);

            //// Assert
            //Assert.IsTrue(result.IsSuccess);
            
        //}

    }
}
