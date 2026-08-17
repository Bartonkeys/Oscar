using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;
using Oscar.Infrastructure.Features.Common;
using Oscar.Infrastructure.Features.Episode.Commands;
using Oscar.Infrastructure.Features.Episode.Queries;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Oscar.Core.Enums;

namespace Oscar.Integration.Tests.Episode
{
    [TestClass]
    public class EpisodeFeatureShouldBeAbleTo: BaseTest
    {


        [ClassInitialize]
        public static void TestFixtureSetup(TestContext context)
        {
            // Executes once for the test class. (Optional)
        }


        [TestMethod]
        public async Task GetEpisodeById()
        {

            // Arrange
            var testId = OscarContext.Episodes.First().Id; 

            // Act
            var getEpisodeByIdQuery = new GetEpisodeByIdQuery();
            getEpisodeByIdQuery.Id = testId;  
            var result = await Mediator.Send(getEpisodeByIdQuery);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Value.Id == testId);
        }

        [TestMethod]
        public async Task GetEpisodeById_InvalidId_ReturnNull()
        {
            // Arrange
            var testId = 1000;

            // Act
            var getEpisodeByIdQuery = new GetEpisodeByIdQuery();
            getEpisodeByIdQuery.Id = testId;
            var result = await Mediator.Send(getEpisodeByIdQuery);

            // Assert
            Assert.IsTrue(result.IsFailure);
        }

        [TestMethod]
        public async Task GetEpisodeById_Id0_ReturnError()
        {
            // Arrange - not needed
            var testId = 0;

            // Act
            var getEpisodeByIdQuery = new GetEpisodeByIdQuery();
            getEpisodeByIdQuery.Id = testId;
            var result = await Mediator.Send(getEpisodeByIdQuery);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.IsFailure);
            Assert.IsTrue(result.Error.Length > 0);
        }


        [TestMethod]
        public async Task AddEpisode()
        {
            // Arrange
            var seasonId = OscarContext.Seasons.First().Id;
            var seriesId = OscarContext.Series.First().Id;
            var contactId = OscarContext.Contacts.First().Id;
            var genreId = OscarContext.Genres.First().Id;
            var directorId = OscarContext.Directors.First().Id;
            var customServiceManagerId = OscarContext.CustomServiceManagers.First().Id;

            var episodeAddDto = new EpisodeAddDto
            {
                Titles = new List<WorksTitleDto>() { new WorksTitleDto() { Title = "test" } },
                GenreId = genreId,
                DurationMinutes = 60,
                ProductionYear = 2000,
                FirstBroadcastYear = 2000,
                IMaestroWorkCode = "MWR001",
                AgicoaWorksReference = "ADN001",
                Isan = "ISAN001",
                CavcoCode = "CCC001",
                GeneralNotes = "General note 001",
                Number = 1000,
                SeasonId = seasonId,
                CountryIds = new List<int>() { 1 },
                DirectorIds = new List<int>(){ directorId },
                CompanyIds = new List<int>() { 1 },
                LanguageIds = new List<int>() { 1 }
            };

            // Act
            var result = await Mediator.Send(new AddEpisodeCommand { EpisodeAddDto = episodeAddDto });

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Value.Id > 0);
        }

        [TestMethod]
        public async Task AddEpisode_NoTitle_ReturnsError()
        {
            // Arrange
            var seasonId = OscarContext.Seasons.First().Id;
            var seriesId = OscarContext.Series.First().Id;
            var contactId = OscarContext.Contacts.First().Id;
            var genreId = OscarContext.Genres.First().Id;
            var customServiceManagerId = OscarContext.CustomServiceManagers.First().Id;

            var episodeAddDto = new EpisodeAddDto
            {
                GenreId = genreId,
                DurationMinutes = 60,
                ProductionYear = 2000,
                FirstBroadcastYear = 2000,
                IMaestroWorkCode = "MWR001",
                AgicoaWorksReference = "ADN001",
                Isan = "ISAN001",
                CavcoCode = "CCC001",
                GeneralNotes = "General note 001",
                Number = 1000,
                SeasonId = seasonId,
                CountryIds = new List<int>() { 1 }

            };

            // Act
            var result = await Mediator.Send(new AddEpisodeCommand { EpisodeAddDto = episodeAddDto });

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.IsFailure);
            Assert.IsTrue(result.Error.Length > 0);
        }



        [TestMethod]
        public async Task AddEpisode_InvalidData_ReturnError()
        {
            // Arrange
            var seasonId = OscarContext.Seasons.First().Id;
            var seriesId = OscarContext.Series.First().Id;
            
            var episodeAddDto = new EpisodeAddDto
            {
                GenreId = 1,
                DurationMinutes = 60,
                ProductionYear = 1, 
                FirstBroadcastYear = 2000,
                IMaestroWorkCode = "MWR001",
                AgicoaWorksReference = "ADN001",
                Isan = "ISAN001",
                CavcoCode = "CCC001",
                GeneralNotes = "General note 001",
                Number = 1000,
                SeasonId = seasonId,
                CountryIds = new List<int>() { 1 }

            };

            // Act
            var result = await Mediator.Send(new AddEpisodeCommand { EpisodeAddDto = episodeAddDto });

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.IsFailure);
            Assert.IsTrue(result.Error.Length > 0);
        }


        [TestMethod]
        public async Task UpdateEpisode()
        {
            // Arrange
            var recordToUpdateId = OscarContext.Episodes.First().Id;
            var seasonId = OscarContext.Seasons.First().Id;
            var seriesId = OscarContext.Series.First().Id;
            var directorId = OscarContext.Directors.First().Id;
            var updated = "updated";

            var episodeUpdateDto = new EpisodeUpdateDto
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
                WorksStatus = Core.Enums.WorksStatus.Active,
                SeasonId = seasonId,
                CountryIds = new List<int>{1},
                DirectorIds = new List<int>{directorId},
                CompanyIds = new List<int>() { 1 },
                LanguageIds = new List<int>() { 1 }
            };

            // Act
            var result = await Mediator.Send(new UpdateEpisodeCommand { EpisodeUpdateDto = episodeUpdateDto, Id = recordToUpdateId });
            var updatedRecord = await Mediator.Send(new GetEpisodeByIdQuery { Id = recordToUpdateId });

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(updated, updatedRecord.Value.IMaestroWorkCode);
            
        }


        [TestMethod]
        public async Task UpdateEpisode_InvalidId_ReturnFalse()
        {
            // Arrange
            var recordToUpdateId = 1000;
            var seasonId = OscarContext.Seasons.First().Id;
            var seriesId = OscarContext.Series.First().Id;
            var updated = "updated";
            var episodeUpdateDto = new EpisodeUpdateDto
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
                SeasonId = seasonId,
                CountryIds = new List<int>{1},
                DirectorIds = new List<int>{1},
                CompanyIds = new List<int>() { 1 },
                LanguageIds= new List<int>() { 1 }
            };

            // Act
            var result = await Mediator.Send(new UpdateEpisodeCommand { EpisodeUpdateDto = episodeUpdateDto, Id = recordToUpdateId });

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(CommandResult.NOTFOUND, result.Error);
        }

        [TestMethod]
        public async Task UpdateEpisode_InvalidData_ReturnError()
        {
            // Arrange
            var recordToUpdateId = 100;
            var updated = "updated";
            var seasonId = OscarContext.Seasons.First().Id;
            var seriesId = OscarContext.Series.First().Id;
            var episodeUpdateDto = new EpisodeUpdateDto
            {
                Titles = new List<WorksTitleDto>() { new WorksTitleDto() { Title = "test" } },
                GenreId = 1,
                DurationMinutes = 61,
                ProductionYear = 1000,
                FirstBroadcastYear = 1000,
                IMaestroWorkCode = updated,
                AgicoaWorksReference = updated,
                Isan = updated,
                CavcoCode = updated,
                GeneralNotes = updated,
                Number = 1001,
                SeasonId = seasonId
            };

            // Act
            var result = await Mediator.Send(new UpdateEpisodeCommand { EpisodeUpdateDto = episodeUpdateDto, Id = recordToUpdateId });

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.IsFailure);
            Assert.IsNotNull(result.Error);
            Assert.AreNotEqual(string.Empty, result.Error);
        }


    }
}
