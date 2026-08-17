using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oscar.Core.DTOs;
using Oscar.Infrastructure.Features.WorksImport.Commands;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Oscar.Integration.Tests.WorksImport
{
    [TestClass]
    public class EpisodeImportFeatureShouldBeAbleTo : BaseTest
    {


        [TestMethod]
        public async Task AddEpisodeImportRequest()
        {
            // Arrange
            var worksImportRequestAddDto = new WorksImportRequestAddDto
            {
                RequestedBy = "Johnny Cash",
                ClientId = 1,
                CatalogueId = 1
            };

            var separator = Path.DirectorySeparatorChar;
            var filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.Replace($"bin{separator}Debug{separator}net6.0", $"{separator}TestFiles{separator}ImportEpisode2.csv"));
            var stream = File.OpenRead(filePath);
            
            worksImportRequestAddDto.FormFile =new FormFile(stream, 0, stream.Length, "ImportEpisode2.csv", Path.GetFileName(stream.Name))
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/csv"
            };

            var addEpisodeImportRequestCommand = new AddEpisodeImportRequestCommand
            {
                WorksImportRequestAddDto = worksImportRequestAddDto
            };

            // Act
            var result = await Mediator.Send(addEpisodeImportRequestCommand);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Value > 0);

        }

        [TestMethod]
        public async Task AddEpisodeImportRequest_InvalidFileReturnsError()
        {
            // Arrange
            var worksImportRequestAddDto = new WorksImportRequestAddDto
            {
                RequestedBy = "Johnny Cash",
                ClientId = 1,
                CatalogueId = 1
            };

            var separator = Path.DirectorySeparatorChar;
            var filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.Replace($"bin{separator}Debug{separator}net6.0", $"{separator}TestFiles{separator}ImportEpisode1.csv"));
            var stream = File.OpenRead(filePath);

            worksImportRequestAddDto.FormFile = new FormFile(stream, 0, stream.Length, "ImportEpisode1.csv", Path.GetFileName(stream.Name))
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/csv"
            };

            var addEpisodeImportRequestCommand = new AddEpisodeImportRequestCommand
            {
                WorksImportRequestAddDto = worksImportRequestAddDto
            };

            // Act
            var result = await Mediator.Send(addEpisodeImportRequestCommand);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.Error.Length > 0);

        }


        [TestMethod]
        public async Task AddEpisodeImportRequest_MissingRequestedBy_ReturnsError()
        {
            // Arrange
            var worksImportRequestAddDto = new WorksImportRequestAddDto
            {
                ClientId = 1,
                CatalogueId = 1
            };

            var separator = Path.DirectorySeparatorChar;
            var filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.Replace($"bin{separator}Debug{separator}net6.0", $"{separator}TestFiles{separator}ImportEpisode2.csv"));
            var stream = File.OpenRead(filePath);

            worksImportRequestAddDto.FormFile =  new FormFile(stream, 0, stream.Length, "ImportEpisode2.csv", Path.GetFileName(stream.Name))
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/csv"
            };

            var addEpisodeImportRequestCommand = new AddEpisodeImportRequestCommand
            {
                WorksImportRequestAddDto = worksImportRequestAddDto
            };

            // Act
            var result = await Mediator.Send(addEpisodeImportRequestCommand);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.IsFailure);
            Assert.IsTrue(result.Error.Length > 0);
        }


        [TestMethod]
        public async Task AddEpisodeImportRequest_MissingFile_ReturnsError()
        {
            // Arrange
            var worksImportRequestAddDto = new WorksImportRequestAddDto
            {
                RequestedBy = "Johnny Cash",
                ClientId = 1,
                CatalogueId = 1
            };

            var addEpisodeImportRequestCommand = new AddEpisodeImportRequestCommand
            {
                WorksImportRequestAddDto = worksImportRequestAddDto
            };

            // Act
            var result = await Mediator.Send(addEpisodeImportRequestCommand);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.IsFailure);
            Assert.IsTrue(result.Error.Length > 0);
        }

    }
}
