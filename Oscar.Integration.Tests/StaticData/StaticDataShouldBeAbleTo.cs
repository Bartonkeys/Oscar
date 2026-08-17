using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Infrastructure.Features.Common;
using Oscar.Infrastructure.Features.Clients.Queries;
using Oscar.Infrastructure.Features.Country.Queries;
using Oscar.Infrastructure.Features.Works.Queries;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Text.Json;


namespace Oscar.Integration.Tests.StaticData
{
    [TestClass]
    public class StaticDataShouldBeAbleTo: BaseTest
    {

        [TestMethod]
        public async Task GetListOfAllEnums()
        {
            // Arrange
            var allEnums = Enum<Enums>.GetAllValuesAsIEnumerable().Select(d => new EnumDTO(d));

            // Act
            var getClientStaticDataQuery = new GetClientStaticDataQuery();
            var result = await Mediator.Send(getClientStaticDataQuery);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(JsonSerializer.Serialize(result.Value), JsonSerializer.Serialize(allEnums));
        }

        [TestMethod]
        public async Task GetClientGradesEnum()
        {
            // Arrange
            var clientGradeEnum = Enum<ClientGrade>.GetAllValuesAsIEnumerable().Select(d => new EnumDTO(d));

            // Act
            var getClientStaticDataQuery = new GetClientStaticDataQuery(Enums.ClientGrade);
            var result = await Mediator.Send(getClientStaticDataQuery);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(JsonSerializer.Serialize(result.Value), JsonSerializer.Serialize(clientGradeEnum));
        }

        [TestMethod]
        public async Task GetStatusesEnum()
        {
            // Arrange
            var statusesEnum = Enum<Status>.GetAllValuesAsIEnumerable().Select(d => new EnumDTO(d));

            // Act
            var getClientStaticDataQuery = new GetClientStaticDataQuery(Enums.Status);
            var result = await Mediator.Send(getClientStaticDataQuery);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(JsonSerializer.Serialize(result.Value), JsonSerializer.Serialize(statusesEnum));
        }

        [TestMethod]
        public async Task GetClientTypeEnum()
        {
            // Arrange
            var clientTypeEnum = Enum<ClientType>.GetAllValuesAsIEnumerable().Select(d => new EnumDTO(d));

            // Act
            var getClientStaticDataQuery = new GetClientStaticDataQuery(Enums.ClientType);
            var result = await Mediator.Send(getClientStaticDataQuery);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(JsonSerializer.Serialize(result.Value), JsonSerializer.Serialize(clientTypeEnum));
        }

        [TestMethod]
        public async Task GetCountries()
        {
            // Arrange
            // {Use data from base test}

            // Act
            var getCountryQuery = new GetCountryQuery();
            var result = await Mediator.Send(getCountryQuery);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Value.Records.ToList().Count() == 5);
            Assert.IsTrue(result.Value.TotalRecords == 5);
        }

        [TestMethod]
        public async Task GetWorksGenre()
        {
            // Arrange
            // {Use data from base test}

            // Act
            var getGenreStaticDataQuery = new GetGenreStaticDataQuery();
            var result = await Mediator.Send(getGenreStaticDataQuery);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Value.ToList().Count() == 5);
        }

        [TestMethod]
        public async Task GetWorksLanguage()
        {
            // Arrange
            // {Use data from base test}

            // Act
            var getLanguageStaticDataQuery = new GetLanguageStaticDataQuery();
            var result = await Mediator.Send(getLanguageStaticDataQuery);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Value.ToList().Count() == 3);
        }

        [TestMethod]
        public async Task GetWorksStatus()
        {
            // Arrange
            var worksStatusEnum = Enum<WorksStatus>.GetAllValuesAsIEnumerable().Select(d => new EnumDTO(d));

            // Act
            var getWorksStaticDataQuery = new GetWorksStaticDataQuery(Enums.WorksStatus);
            var result = await Mediator.Send(getWorksStaticDataQuery);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(JsonSerializer.Serialize(result.Value), JsonSerializer.Serialize(worksStatusEnum));

        }


    }
}
