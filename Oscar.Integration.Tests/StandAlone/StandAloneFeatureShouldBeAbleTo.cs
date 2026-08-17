using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oscar.Core.DTOs;
using Oscar.Infrastructure.Features.Common;
using Oscar.Infrastructure.Features.StandAlone.Commands;
using Oscar.Infrastructure.Features.StandAlone.Queries;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Oscar.Core.Enums;

namespace Oscar.Integration.Tests.StandAlone
{
    [TestClass]
    public class StandAloneFeatureShouldBeAbleTo: BaseTest
    {
        

        [TestMethod]
        public async Task GetStandAloneById()
        {
            // Arrange
            var testId = OscarContext.StandAlones.First().Id;

            // Act
            var getStandAloneByIdQuery = new GetStandAloneByIdQuery();
            getStandAloneByIdQuery.Id = testId;  
            var result = await Mediator.Send(getStandAloneByIdQuery);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Value.Id == testId);
        }


        [TestMethod]
        public async Task GetStandAloneById_InvalidId_ReturnNull()
        {
            // Arrange
            var testId = 1000000;

            // Act
            var getStandAloneByIdQuery = new GetStandAloneByIdQuery();
            getStandAloneByIdQuery.Id = testId;
            var result = await Mediator.Send(getStandAloneByIdQuery);

            // Assert
            Assert.IsTrue(result.IsFailure);
        }


        [TestMethod]
        public async Task GetStandAloneById_Id0_ReturnError()
        {
            // Arrange
            var testId = 0;

            // Act
            var getStandAloneByIdQuery = new GetStandAloneByIdQuery();
            getStandAloneByIdQuery.Id = testId;
            var result = await Mediator.Send(getStandAloneByIdQuery);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.IsFailure);
            Assert.IsTrue(result.Error.Length > 0);
        }


        [TestMethod]
        public async Task AddStandAlone()
        {
            // Arrange

            var standAloneAddDto = new StandAloneAddDto
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
                CountryIds = new List<int> { 1 },
                DirectorIds = new List<int>() { 1 },
                CompanyIds = new List<int>() { 1 },
                LanguageIds = new List<int>() { 1 }
            };

            // Act
            var result = await Mediator.Send(new AddStandAloneCommand { StandAloneAddDto = standAloneAddDto });

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Value.Id > 0);
        }


        [TestMethod]
        public async Task AddStandAlone_NoTitle_ReturnsError()
        {
            // Arrange

            var standAloneAddDto = new StandAloneAddDto
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
                Number = 1000
            };

            // Act
            var result = await Mediator.Send(new AddStandAloneCommand { StandAloneAddDto = standAloneAddDto });

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.IsFailure);
            Assert.IsTrue(result.Error.Length > 0);
        }


        [TestMethod]
        public async Task AddStandAlone_InvalidData_ReturnError()
        {
            // Arrange
            var standAloneAddDto = new StandAloneAddDto
            {
                WorksStatus = Core.Enums.WorksStatus.Active,
                GenreId = 1,
                DurationMinutes = 60,
                ProductionYear = 1, //Should not allow a production year before 1900
                FirstBroadcastYear = 2000, 
                IMaestroWorkCode = "MWR001",
                AgicoaWorksReference = "ADN001",
                Isan = "ISAN001",
                CavcoCode = "CCC001",
                GeneralNotes = "General note 001",
                Number = 1000
            };

            // Act
            var result = await Mediator.Send(new AddStandAloneCommand { StandAloneAddDto = standAloneAddDto });

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.IsFailure);
            Assert.IsTrue(result.Error.Length > 0);
        }


        [TestMethod]
        public async Task UpdateStandAlone()
        {
            // Arrange
            var recordToUpdateId = OscarContext.StandAlones.First().Id;
            var updated = "updated";
            var standAloneUpdateDto = new StandAloneUpdateDto
            {
                Titles = new List<WorksTitleDto>() { new WorksTitleDto() { Title = "test", TitleType = TitleType.Main } },
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
                ClientIds = new List<int>() { 1 },
                ClientReferences = new List<ClientReferenceDto>() { new ClientReferenceDto() { ClientId = 1 } },
                CatalogueIds = new List<int>() { 1 },
                CountryIds = new List<int> { 1 },
                DirectorIds = new List<int>() { 1 },
                CompanyIds = new List<int>() { 1 },
                LanguageIds = new List<int>() { 1 }
            };

            // Act
            var result = await Mediator.Send(new UpdateStandAloneCommand { StandAloneUpdateDto = standAloneUpdateDto, Id = recordToUpdateId });
            var updatedRecord = await Mediator.Send(new GetStandAloneByIdQuery { Id = recordToUpdateId });

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(updated, updatedRecord.Value.IMaestroWorkCode);

        }


        [TestMethod]
        public async Task UpdateStandAlone_InvalidId_ReturnFalse()
        {
            // Arrange
            var recordToUpdateId = 1000000;
            var updated = "updated";
            var standAloneUpdateDto = new StandAloneUpdateDto
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
                ClientIds = new List<int>(){1},
                ClientReferences = new List<ClientReferenceDto>() { new ClientReferenceDto() { ClientId = 1 } },
                CatalogueIds = new List<int>(){1},
                CountryIds = new List<int> { 1 },
                DirectorIds = new List<int>() { 1 },
                CompanyIds = new List<int>() { 1 },
                LanguageIds= new List<int> { 1 }
            };

            // Act
            var result = await Mediator.Send(new UpdateStandAloneCommand { StandAloneUpdateDto = standAloneUpdateDto, Id = recordToUpdateId });

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(CommandResult.NOTFOUND, result.Error);
        }

        [TestMethod]
        public async Task UpdateStandAlone_InvalidData_ReturnError()
        {
            // Arrange
            var recordToUpdateId = 1;
            var updated = "updated";
            var standAloneUpdateDto = new StandAloneUpdateDto
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
                Number = 1001
            };

            // Act
            var result = await Mediator.Send(new UpdateStandAloneCommand { StandAloneUpdateDto = standAloneUpdateDto, Id = recordToUpdateId });

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.IsFailure);
            Assert.IsNotNull(result.Error);
            Assert.AreNotEqual(string.Empty, result.Error);
        }


    }
}
