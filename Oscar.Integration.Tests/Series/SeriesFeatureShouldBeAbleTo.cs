using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oscar.Core.DTOs;
using Oscar.Infrastructure.Features.Common;
using Oscar.Infrastructure.Features.Series.Commands;
using Oscar.Infrastructure.Features.Series.Queries;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Oscar.Core.Enums;

namespace Oscar.Integration.Tests.Series
{
    [TestClass]
    public class SeriesFeatureShouldBeAbleTo: BaseTest
    {
        
        [TestMethod]
        public async Task GetSeriesById()
        {
            // Arrange
            var testId = OscarContext.Series.First().Id;

            // Act
            var getSeriesByIdQuery = new GetSeriesByIdQuery();
            getSeriesByIdQuery.Id = testId;  
            var result = await Mediator.Send(getSeriesByIdQuery);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Value.Id == testId);
        }

        [TestMethod]
        public async Task GetSeriesById_InvalidId_ReturnNull()
        {
            // Arrange
            var testId = 1000;

            // Act
            var getSeriesByIdQuery = new GetSeriesByIdQuery();
            getSeriesByIdQuery.Id = testId;
            var result = await Mediator.Send(getSeriesByIdQuery);

            // Assert
            Assert.IsTrue(result.IsFailure);
        }

        [TestMethod]
        public async Task GetSeriesById_Id0_ReturnError()
        {
            // Arrange - not needed
            var testId = 0;

            // Act
            var getSeriesByIdQuery = new GetSeriesByIdQuery();
            getSeriesByIdQuery.Id = testId;
            var result = await Mediator.Send(getSeriesByIdQuery);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.IsFailure);
            Assert.IsTrue(result.Error.Length > 0);
        }


        [TestMethod]
        public async Task AddSeries()
        {
            // Arrange

            var seriesAddDto = new SeriesAddDto
            {
                Titles = new List<WorksTitleDto>() { new WorksTitleDto() { Title = "test" } },
                GenreId = 1,
                DurationMinutes = 60,
                ProductionYear = 2000,
                FirstBroadcastYear = 2000,
                IMaestroWorkCode = "MWR001",
                AgicoaWorksReference = "ADN001",
                Isan = "ISAN001",
                CavcoCode = "CCC001",
                GeneralNotes = "General note 001",
                Number = 1000,
                CountryIds = new List<int>() { 1 },
                DirectorIds = new List<int>() { 1 },
                CompanyIds = new List<int>() { 1 },
                LanguageIds = new List<int>() { 1 }
            };

            // Act
            var result = await Mediator.Send(new AddSeriesCommand { SeriesAddDto = seriesAddDto });

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Value.Id > 0);
        }


        [TestMethod]
        public async Task AddSeries_NoTitle_ReturnsError()
        {
            // Arrange

            var seriesAddDto = new SeriesAddDto
            {
                GenreId = 1,
                DurationMinutes = 60,
                ProductionYear = 2000,
                FirstBroadcastYear = 2000,
                IMaestroWorkCode = "MWR001",
                AgicoaWorksReference = "ADN001",
                Isan = "ISAN001",
                CavcoCode = "CCC001",
                GeneralNotes = "General note 001",
                Number = 1000,
                CountryIds = new List<int>() { 1 }

            };

            // Act
            var result = await Mediator.Send(new AddSeriesCommand { SeriesAddDto = seriesAddDto });

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.IsFailure);
            Assert.IsTrue(result.Error.Length > 0);
        }



        [TestMethod]
        public async Task AddSeries_InvalidData_ReturnError()
        {
            // Arrange
            var seriesAddDto = new SeriesAddDto
            {
                WorksStatus = Core.Enums.WorksStatus.Active,
                GenreId = 100,
                DurationMinutes = 60,
                ProductionYear = 1999, 
                FirstBroadcastYear = 2000,
                IMaestroWorkCode = "MWR001",
                AgicoaWorksReference = "ADN001",
                Isan = "ISAN001",
                CavcoCode = "CCC001",
                GeneralNotes = "General note 001",
                Number = 1000,
                CountryIds = new List<int>() { 1 }

            };

            // Act
            var result = await Mediator.Send(new AddSeriesCommand { SeriesAddDto = seriesAddDto });

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.IsFailure);
            Assert.IsTrue(result.Error.Length > 0);
        }


        [TestMethod]
        public async Task UpdateSeries()
        {
            // Arrange
            var recordToUpdateId = OscarContext.Series.First().Id;
            var updated = "updated";
            var seriesUpdateDto = new SeriesUpdateDto
            {
                Titles = new List<WorksTitleDto>() { new WorksTitleDto() { Title = "test" } },
                GenreId = 1,
                DurationMinutes = 61,
                ProductionYear = 2001,
                FirstBroadcastYear = 2001,
                IMaestroWorkCode = updated,
                AgicoaWorksReference = updated,
                Isan = updated,
                CavcoCode = updated,
                GeneralNotes = updated,
                Number = 1001,
                WorksStatus = WorksStatus.Active,
                CountryIds = new List<int>() { 1 },
                DirectorIds = new List<int>() { 1 },
                CompanyIds = new List<int>() { 1 },
                LanguageIds = new List<int>() { 1 }
            };

            // Act
            var result = await Mediator.Send(new UpdateSeriesCommand { SeriesUpdateDto = seriesUpdateDto, Id = recordToUpdateId });
            var updatedRecord = await Mediator.Send(new GetSeriesByIdQuery { Id = recordToUpdateId });

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(updated, updatedRecord.Value.IMaestroWorkCode);
            
        }


        [TestMethod]
        public async Task UpdateSeries_InvalidId_ReturnFalse()
        {
            // Arrange

            var recordToUpdateId = 100;
            var updated = "updated";
            var seriesUpdateDto = new SeriesUpdateDto
            {
                Titles = new List<WorksTitleDto>() { new WorksTitleDto() { Title = "test" } },
                GenreId = 2,
                DurationMinutes = 61,
                ProductionYear = 2001,
                FirstBroadcastYear = 2001,
                IMaestroWorkCode = updated,
                AgicoaWorksReference = updated,
                Isan = updated,
                CavcoCode = updated,
                GeneralNotes = updated,
                Number = 1001,
                CountryIds = new List<int>() { 1 },
                DirectorIds = new List<int>() { 1 },
                CompanyIds = new List<int>() { 1 },
                LanguageIds= new List<int>() { 1 }
            };

            // Act
            var result = await Mediator.Send(new UpdateSeriesCommand { SeriesUpdateDto = seriesUpdateDto, Id = recordToUpdateId });

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(CommandResult.NOTFOUND, result.Error);
        }

        [TestMethod]
        public async Task UpdateSeries_InvalidData_ReturnError()
        {
            // Arrange
            var recordToUpdateId = 1;
            var updated = "updated";
            var seriesUpdateDto = new SeriesUpdateDto
            {
                WorksStatus = 0,
                GenreId = 0,
                DurationMinutes = 61,
                ProductionYear = 1000,
                FirstBroadcastYear = 1000,
                IMaestroWorkCode = updated,
                AgicoaWorksReference = updated,
                Isan = updated,
                CavcoCode = updated,
                GeneralNotes = updated,
                Number = 1001,
                CountryIds = new List<int>() { 1 }
            };

            // Act
            var result = await Mediator.Send(new UpdateSeriesCommand { SeriesUpdateDto = seriesUpdateDto, Id = recordToUpdateId });

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.IsFailure);
            Assert.IsNotNull(result.Error);
            Assert.AreNotEqual(string.Empty, result.Error);
        }

    }
}
