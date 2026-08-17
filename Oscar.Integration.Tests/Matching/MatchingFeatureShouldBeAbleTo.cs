using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Infrastructure.Features.Matching.Commands;
using Oscar.Infrastructure.Features.Matching.Queries;
using Oscar.Infrastructure.Features.Matching.Services;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Oscar.Integration.Tests.Matching
{
    [TestClass]
    public class MatchingFeatureShouldBeAbleTo : BaseTest
    {
        


        [TestMethod]
        public async Task AddMatching()
        {
            // Arrange
            var fileMock = MockFile("test.csv");

            var rules = MatchRules.TitleCheckLevel1 | MatchRules.TitleCheckLevel2;

            var matchingRequestAddDto = new MatchRequestAddDto
            {
                FormFile = fileMock.Object,
                Rules = rules,
                RequestedBy = "Joey Ramone",
                RightsFromYear = 1999,
                RightsToYear = 2004,
                TerritoryId = 1,
                IgnoreCharactersFollowing = ":"
            };

            var addMatchRequestCommand = new AddMatchRequestCommand
            {
                MatchRequestAddDto = matchingRequestAddDto
            };

            // Act
            var result = await Mediator.Send(addMatchRequestCommand);
            var matchRequest = OscarContext.MatchRequests.OrderByDescending(x => x.Id).First();

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(matchingRequestAddDto.RequestedBy, matchRequest.RequestedBy);
            Assert.AreEqual(MatchRequestStatus.Pending, matchRequest.Status);
            Assert.AreNotEqual(0, matchRequest.Reference.Length);

        }


        [TestMethod]
        public async Task AddMatching_MissingRules_ReturnsError()
        {
            // Arrange
            var fileMock = MockFile("test.csv");

            var matchingRequestAddDto = new MatchRequestAddDto
            {
                FormFile = fileMock.Object,
                RequestedBy = "Joey Ramone"
            };

            var addMatchRequestCommand = new AddMatchRequestCommand
            {
                MatchRequestAddDto = matchingRequestAddDto
            };

            // Act
            var result = await Mediator.Send(addMatchRequestCommand);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.IsFailure);
            Assert.IsTrue(result.Error.Length > 0);

        }


        [TestMethod]
        public async Task AddMatching_MissingRequestedBy_ReturnsError()
        {
            // Arrange
            var fileMock = MockFile("test.csv");

            var rules = MatchRules.TitleCheckLevel1 | MatchRules.TitleCheckLevel2;

            var matchingRequestAddDto = new MatchRequestAddDto
            {
                FormFile = fileMock.Object,
                Rules = rules
            };

            var addMatchRequestCommand = new AddMatchRequestCommand
            {
                MatchRequestAddDto = matchingRequestAddDto
            };

            // Act
            var result = await Mediator.Send(addMatchRequestCommand);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.IsFailure);
            Assert.IsTrue(result.Error.Length > 0);
        }


        [TestMethod]
        public async Task AddMatching_MissingFile_ReturnsError()
        {
            // Arrange
            var rules = MatchRules.TitleCheckLevel1 | MatchRules.TitleCheckLevel2;

            var matchingRequestAddDto = new MatchRequestAddDto
            {
                RequestedBy = "Joey Ramone",
                Rules = rules
            };

            var addMatchRequestCommand = new AddMatchRequestCommand
            {
                MatchRequestAddDto = matchingRequestAddDto
            };

            // Act
            var result = await Mediator.Send(addMatchRequestCommand);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.IsFailure);
            Assert.IsTrue(result.Error.Length > 0);
        }


        [TestMethod]
        public async Task AddMatching_WrongFileExtension_ReturnsError()
        {
            // Arrange
            var fileMock = MockFile("test.doc");

            var rules = MatchRules.TitleCheckLevel1 | MatchRules.TitleCheckLevel2;

            var matchingRequestAddDto = new MatchRequestAddDto
            {
                FormFile = fileMock.Object,
                Rules = rules,
                RequestedBy = "Joey Ramone"
            };

            var addMatchRequestCommand = new AddMatchRequestCommand
            {
                MatchRequestAddDto = matchingRequestAddDto
            };

            // Act
            var result = await Mediator.Send(addMatchRequestCommand);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.IsFailure);
            Assert.IsTrue(result.Error.Length > 0);

        }

        [TestMethod]
        public async Task GetMatch()
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write("sample data");
            writer.Flush();
            stream.Position = 0;

            BlobsModelFactory.BlobDownloadStreamingResult(stream);

            // Act
            var request = new GetMatchingQuery();
            var searchObject = new SearchObject("MatchRequest", "string", "SearchText", "BK_oscar_match_test.csv");
            request.SearchObjects.Add(searchObject);
            var result = await Mediator.Send(request);

            // Assert
            Assert.IsTrue(result.IsSuccess);
        }


        //[TestMethod]
        //public async Task Match()
        //{
        //    // Arrange
        //    var matchCommand = new MatchCommand
        //    {
        //        Reference = "TEST_REF_01"
        //    };

        //    // Act
        //    var result = await Mediator.Send(matchCommand);

        //    // Assert
        //    Assert.IsTrue(result.IsSuccess);

        //}


        //[TestMethod]
        //public async Task Match_ThrowsErrorOnRulesNotSet()
        //{
        //    // Arrange
        //    var matchRequest = OscarContext.MatchRequests.FirstOrDefault(m => m.Reference == "TEST_REF_01");
        //    var importResult = Importer.ImportMatchCsvAsList(matchRequest.Reference + ".csv");

        //    Assert.ThrowsException<MatchingServiceRulesNotSetException>( () =>
        //    {
        //        var result = MatchingService.Match(importResult.Value[0]);
        //    });
        //}


        //[TestMethod]
        //public async Task Match_ThrowsErrorOnClientNotSet()
        //{
        //    // Arrange
        //    var matchRequest = OscarContext.MatchRequests.FirstOrDefault(m => m.Reference == "TEST_REF_01");
        //    var importResult = Importer.ImportMatchCsvAsList(matchRequest.Reference + ".csv");

        //    MatchingService.LoadRules(
        //        matchRequest.Rules,
        //        null,
        //        matchRequest.TerritoryId,
        //        matchRequest.ProductionYear,
        //        matchRequest.RightsTypeId,
        //        matchRequest.RightsFromYear,
        //        matchRequest.RightsToYear,
        //        matchRequest.IgnoreCharactersFollowing);

        //    Assert.ThrowsException<MatchingServiceRulesNotSetException>(() =>
        //    {
        //        var result = MatchingService.Match(importResult.Value[0]);
        //    });
        //}

        //[TestMethod]
        //public async Task Match_SetMismatches()
        //{
        //    // Arrange
        //    var matchRequest = OscarContext.MatchRequests.FirstOrDefault(m => m.Reference == "TEST_REF_02");
        //    var importResult = Importer.ImportMatchCsvAsList(matchRequest.Reference + ".csv");

        //    MatchingService.LoadRules(
        //        matchRequest.Rules,
        //        matchRequest.ClientId,
        //        matchRequest.TerritoryId,
        //        matchRequest.ProductionYear,
        //        matchRequest.RightsTypeId,
        //        matchRequest.RightsFromYear,
        //        matchRequest.RightsToYear,
        //        matchRequest.IgnoreCharactersFollowing);

            
        //    var result = await MatchingService.Match(importResult.Value[0]);

        //    Assert.IsTrue(result.IsSuccess);
        //    Assert.IsNotNull(result.Value.MatchingIssue);
        //    Assert.IsTrue(result.Value.MatchingIssue.Contains(Mismatch.TerritoryRightsMismatch));
        //    Assert.IsTrue(result.Value.MatchingIssue.Contains(Mismatch.ProductionYearMismatch));
        //    Assert.IsTrue(result.Value.MatchingIssue.Contains(Mismatch.RightsFromAndToYearMismatch));
        //    Assert.IsTrue(result.Value.MatchingIssue.Contains(Mismatch.RightsTypeMismatch));
        //    Assert.IsTrue(result.Value.MatchingIssue.Contains(Mismatch.DirectorMismatch));
        //    Assert.IsTrue(result.Value.MatchingIssue.Contains(Mismatch.DurationMismatch));
        //    Assert.IsTrue(result.Value.MatchingIssue.Contains(Mismatch.ProductionCountryMismatch));
        //}

        //[TestMethod]
        //public async Task Match_NoMismatches()
        //{
        //    // Arrange
        //    var matchRequest = OscarContext.MatchRequests.FirstOrDefault(m => m.Reference == "TEST_REF_01");
        //    var importResult = Importer.ImportMatchCsvAsList(matchRequest.Reference + ".csv");
        //    var client = OscarContext.Clients.FirstOrDefault(c => c.ClientName == "Test client one");


        //    MatchingService.LoadRules(
        //        matchRequest.Rules,
        //        client?.Id,
        //        1,
        //        matchRequest.ProductionYear,
        //        matchRequest.RightsTypeId,
        //        matchRequest.RightsFromYear,
        //        matchRequest.RightsToYear,
        //        matchRequest.IgnoreCharactersFollowing);


        //    var result = MatchingService.Match(importResult.Value[0]);

        //    Assert.IsTrue(result.IsSuccess);
        //    Assert.IsNotNull(result.Value.MatchingIssue);
        //    Assert.AreEqual("Territory rights mismatch", result.Value.MatchingIssue);

        //    /*Territory rights mismatch; 
        //     * Production year mismatch; 
        //     * Rights from and to year mismatch; 
        //     * Duration mismatch; Production country mismatch*/

        //}

      
        [TestMethod]
        public async Task GetMatchRequestById()
        {

            // Arrange
            var testId = OscarContext.MatchRequests.First().Id;

            // Act
            var getMatchRequestByIdQuery = new GetMatchRequestByIdQuery();
            getMatchRequestByIdQuery.Id = testId;
            var result = await Mediator.Send(getMatchRequestByIdQuery);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Value.Id == testId);
        }

        [TestMethod]
        public async Task GetMatchRequestById_InvalidId_ReturnNull()
        {
            // Arrange
            var testId = 1000;

            // Act
            var getMatchRequestByIdQuery = new GetMatchRequestByIdQuery();
            getMatchRequestByIdQuery.Id = testId;
            var result = await Mediator.Send(getMatchRequestByIdQuery);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Value == null);
        }

        [TestMethod]
        public async Task GetMatchRequestById_Id0_ReturnError()
        {
            // Arrange - not needed
            var testId = 0;

            // Act
            var getMatchRequestByIdQuery = new GetMatchRequestByIdQuery();
            getMatchRequestByIdQuery.Id = testId;
            var result = await Mediator.Send(getMatchRequestByIdQuery);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.IsFailure);
            Assert.IsTrue(result.Error.Length > 0);
        }


        [TestMethod]
        public async Task GetMatchRequests()
        {
            // Act
            var getMatchRequestsQuery = new GetMatchRequestsQuery();
            var result = await Mediator.Send(getMatchRequestsQuery);

            // Assert
            Assert.IsTrue(result.IsSuccess);
        }

        [TestMethod]
        public async Task GetMatchRequests_Paging()
        {
            // Arrange
            var recordsToTake = 2;
            var recordToStart = 0;

            // Act
            var getMatchRequestsQuery = new GetMatchRequestsQuery();
            getMatchRequestsQuery.Start = recordToStart;
            getMatchRequestsQuery.Take = recordsToTake;
            var result = await Mediator.Send(getMatchRequestsQuery);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(recordsToTake, result.Value.Records.Count());
        }

        [TestMethod]
        public async Task GetMatchRequests_Searching()
        {
            // Arrange
            var searchUser = "TestUser1";

            // Act
            var getMatchRequestsQuery = new GetMatchRequestsQuery();
            getMatchRequestsQuery.SearchObjects.Add(new SearchObject("MatchRequest", "string", "RequestedBy", searchUser));
            var result = await Mediator.Send(getMatchRequestsQuery);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Value.Records.All(r => r.RequestedBy == searchUser));
        }

        [TestMethod]
        public async Task GetMatchRequests_Searching_InvalidColumn_ReturnsError()
        {
            // Arrange
            var searchUser = "TestUser1";

            // Act
            var getMatchRequestsQuery = new GetMatchRequestsQuery();
            getMatchRequestsQuery.SearchObjects.Add(new SearchObject("MatchRequest", "string", "MadeUpBy", searchUser));
            var result = await Mediator.Send(getMatchRequestsQuery);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.IsFailure);
            Assert.IsTrue(result.Error.Length > 0);
        }

        [TestMethod]
        public async Task GetMatchRequests_Sorting_InvalidColumn_ReturnsError()
        {
            // Arrange
            var sortColumn = "MadeUpColumn";

            // Act
            var getMatchRequestsQuery = new GetMatchRequestsQuery();
            getMatchRequestsQuery.SortColumn = sortColumn;
            var result = await Mediator.Send(getMatchRequestsQuery);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.IsFailure);
            Assert.IsTrue(result.Error.Length > 0);
        }

        [TestMethod]
        public async Task GetMatchRequests_Sorting_InvalidDirection_ReturnsError()
        {
            // Arrange
            var sortDirection = "Sideways";
            var sortColumn = "RequestedBy";

            // Act
            var getMatchRequestsQuery = new GetMatchRequestsQuery();
            getMatchRequestsQuery.SortDirection = sortDirection;
            getMatchRequestsQuery.SortColumn = sortColumn;
            var result = await Mediator.Send(getMatchRequestsQuery);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.IsFailure);
            Assert.IsTrue(result.Error.Length > 0);
        }
       

        [TestMethod]
        public async Task GetMatchResultById()
        {
            // Arrange
            var testId = OscarContext.MatchRequests.First().Id;

            // Act
            var getMatchResultByIdQuery = new GetMatchResultByIdQuery();
            getMatchResultByIdQuery.Id = testId;
            var result = await Mediator.Send(getMatchResultByIdQuery);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Value.FileBytes.Count() > 0);
            Assert.IsTrue(result.Value.Id == testId);
        }

        [TestMethod]
        public async Task GetMatchResultById_InvalidId_ReturnNull()
        {
            // Arrange
            var testId = 1000;

            // Act
            var getMatchResultByIdQuery = new GetMatchResultByIdQuery();
            getMatchResultByIdQuery.Id = testId;
            var result = await Mediator.Send(getMatchResultByIdQuery);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Value == null);
        }

        [TestMethod]
        public async Task GetMatchResultById_Id0_ReturnError()
        {
            // Arrange - not needed
            var testId = 0;

            // Act
            var getMatchResultByIdQuery = new GetMatchResultByIdQuery();
            getMatchResultByIdQuery.Id = testId;
            var result = await Mediator.Send(getMatchResultByIdQuery);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.IsFailure);
            Assert.IsTrue(result.Error.Length > 0);
        }

    }
}
