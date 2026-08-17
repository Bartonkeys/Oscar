using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oscar.Core.DTOs;
using Oscar.Infrastructure.Features.Common;
using Oscar.Infrastructure.Features.Season.Commands;
using Oscar.Infrastructure.Features.Season.Queries;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Oscar.Core.Enums;

namespace Oscar.Integration.Tests.Season
{
    [TestClass]
    public class SeasonFeatureShouldBeAbleTo: BaseTest
    {
        
        [TestMethod]
        public async Task GetSeasonById()
        {
            // Arrange
            var testId = OscarContext.Seasons.First().Id;

            // Act
            var getSeasonByIdQuery = new GetSeasonByIdQuery();
            getSeasonByIdQuery.Id = testId;  
            var result = await Mediator.Send(getSeasonByIdQuery);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Value.Id == testId);
        }

        [TestMethod]
        public async Task GetSeasonById_InvalidId_ReturnNull()
        {
            // Arrange
            var testId = 1000;

            // Act
            var getSeasonByIdQuery = new GetSeasonByIdQuery();
            getSeasonByIdQuery.Id = testId;
            var result = await Mediator.Send(getSeasonByIdQuery);

            // Assert
            Assert.IsTrue(result.IsFailure);
         
        }

        [TestMethod]
        public async Task GetSeasonById_Id0_ReturnError()
        {
            // Arrange - not needed
            var testId = 0;

            // Act
            var getSeasonByIdQuery = new GetSeasonByIdQuery();
            getSeasonByIdQuery.Id = testId;
            var result = await Mediator.Send(getSeasonByIdQuery);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.IsFailure);
            Assert.IsTrue(result.Error.Length > 0);
        }


        [TestMethod]
        public async Task AddSeason()
        {
            // Arrange
            var seriesId = OscarContext.Series.First().Id;

            var seasonAddDto = new SeasonAddDto
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
                SeriesId = seriesId,
                CountryIds = new List<int>() { 1 },
                DirectorIds = new List<int>() { 1 },
                CompanyIds = new List<int>() { 1 },
                LanguageIds = new List<int>() { 1 }
            };

            // Act
            var result = await Mediator.Send(new AddSeasonCommand { SeasonAddDto = seasonAddDto });

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Value.Id > 0);
        }

        [TestMethod]
        public async Task AddSeason_NoTitle_ReturnsError()
        {
            // Arrange
            var seriesId = OscarContext.Series.First().Id;

            var seasonAddDto = new SeasonAddDto
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
                SeriesId = seriesId,
                CountryIds = new List<int>() { 1 }
            };

            // Act
            var result = await Mediator.Send(new AddSeasonCommand { SeasonAddDto = seasonAddDto });

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.IsFailure);
            Assert.IsTrue(result.Error.Length > 0);
        }



        [TestMethod]
        public async Task AddSeason_InvalidData_ReturnError()
        {
            // Arrange
            var seasonAddDto = new SeasonAddDto
            {
                GenreId = 1,
                DurationMinutes = 60,
                ProductionYear = 1, //Should not allow a production year before 1900
                FirstBroadcastYear = 2000,
                IMaestroWorkCode = "MWR001",
                AgicoaWorksReference = "ADN001",
                Isan = "ISAN001",
                CavcoCode = "CCC001",
                GeneralNotes = "General note 001",
                Number = 1000,
                SeriesId = 1,
                CountryIds = new List<int>() { 1 }
            };

            // Act
            var result = await Mediator.Send(new AddSeasonCommand { SeasonAddDto = seasonAddDto });

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.IsFailure);
            Assert.IsTrue(result.Error.Length > 0);
        }


        [TestMethod]
        public async Task UpdateSeason()
        {
            // Arrange
            var recordToUpdateId = OscarContext.Seasons.First().Id;
            var seriesId = OscarContext.Series.First().Id;
            var updated = "updated";
            var seasonUpdateDto = new SeasonUpdateDto
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
                WorksStatus = WorksStatus.Active,
                SeriesId = seriesId,
                CountryIds = new List<int> { 1 },
                DirectorIds = new List<int>() { 1 },
                CompanyIds = new List<int>() { 1 },
                LanguageIds = new List<int>() { 1 }
            };

            // Act
            var result = await Mediator.Send(new UpdateSeasonCommand { SeasonUpdateDto = seasonUpdateDto, Id = recordToUpdateId });
            var updatedRecord = await Mediator.Send(new GetSeasonByIdQuery { Id = recordToUpdateId });

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(updated, updatedRecord.Value.IMaestroWorkCode);
            
        }


        [TestMethod]
        public async Task UpdateSeason_InvalidId_ReturnFalse()
        {
            // Arrange

            var recordToUpdateId = 1000;
            var seriesId = OscarContext.Series.First().Id;
            var updated = "updated";
            var seasonUpdateDto = new SeasonUpdateDto
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
                SeriesId = seriesId,
                CountryIds = new List<int> { 1 },
                DirectorIds = new List<int>() { 1 },
                CompanyIds = new List<int>() { 1 },
                LanguageIds= new List<int>() { 1 },
            };

            // Act
            var result = await Mediator.Send(new UpdateSeasonCommand { SeasonUpdateDto = seasonUpdateDto, Id = recordToUpdateId });

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(CommandResult.NOTFOUND, result.Error);
        }

        [TestMethod]
        public async Task UpdateSeason_InvalidData_ReturnError()
        { 
            // Arrange
            var recordToUpdateId = OscarContext.Seasons.First().Id;
            var seriesId = OscarContext.Series.First().Id;
            var updated = "updated";
            var seasonUpdateDto = new SeasonUpdateDto
            {
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
                SeriesId = seriesId
            };

            // Act
            var result = await Mediator.Send(new UpdateSeasonCommand { SeasonUpdateDto = seasonUpdateDto, Id = recordToUpdateId });

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.IsFailure);
            Assert.IsNotNull(result.Error);
            Assert.AreNotEqual(string.Empty, result.Error);
        }


    }
}
