using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Infrastructure.Features.WorksImport.Commands;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Oscar.Integration.Tests.WorksImport
{
    [TestClass]
    public class WorksImportFeatureShouldBeAbleTo : BaseTest
    {


        [TestMethod]
        public async Task AddWorksImportRequest()
        {
            // Arrange
            var worksImportRequestAddDto = new WorksImportRequestAddDto
            {
                RequestedBy = "Johnny Cash",
                ClientId = 1,
                CatalogueId = 1
            };

            var separator = Path.DirectorySeparatorChar;
            var filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.Replace($"bin{separator}Debug{separator}net6.0", $"{separator}TestFiles{separator}TestWorksFile.csv"));
            var stream = File.OpenRead(filePath);
            
            worksImportRequestAddDto.FormFile = new FormFile(stream, 0, stream.Length, "TestWorksFile.csv", Path.GetFileName(stream.Name))
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/csv"
            };
            


            var addWorksImportRequestCommand = new AddWorksImportRequestCommand
            {
                WorksImportRequestAddDto = worksImportRequestAddDto
            };

            // Act
            var result = await Mediator.Send(addWorksImportRequestCommand);
            var worksImportRequest = OscarContext.WorksImportRequests.OrderByDescending(x => x.Id).First();

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(worksImportRequestAddDto.RequestedBy, worksImportRequest.RequestedBy);
            Assert.AreEqual(WorksImportRequestStatus.Pending, worksImportRequest.Status);

        }
        
        [TestMethod]
        public async Task AddWorksImportRequest_MissingRequestedBy_ReturnsError()
        {
            // Arrange
            var worksImportRequestAddDto = new WorksImportRequestAddDto
            {
                ClientId = 1,
                CatalogueId = 1
            };

            var separator = Path.DirectorySeparatorChar;
            var filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.Replace($"bin{separator}Debug{separator}net6.0", $"{separator}TestFiles{separator}TestWorksFile.csv"));
            var stream = File.OpenRead(filePath);

            worksImportRequestAddDto.FormFile = new FormFile(stream, 0, stream.Length, "TestWorksFile.csv", Path.GetFileName(stream.Name))
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/csv"
            };

            var addWorksImportRequestCommand = new AddWorksImportRequestCommand
            {
                WorksImportRequestAddDto = worksImportRequestAddDto
            };

            // Act
            var result = await Mediator.Send(addWorksImportRequestCommand);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.IsFailure);
            Assert.IsTrue(result.Error.Length > 0);
        }


        [TestMethod]
        public async Task AddWorksImportRequest_MissingFile_ReturnsError()
        {
            // Arrange
            var worksImportRequestAddDto = new WorksImportRequestAddDto
            {
                RequestedBy = "Johnny Cash",
                ClientId = 1,
                CatalogueId = 1
            };

            var addWorksImportRequestCommand = new AddWorksImportRequestCommand
            {
                WorksImportRequestAddDto = worksImportRequestAddDto
            };

            // Act
            var result = await Mediator.Send(addWorksImportRequestCommand);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.IsFailure);
            Assert.IsTrue(result.Error.Length > 0);
        }

        //[TestMethod]
        //public async Task UpdateWorksImportRequest()
        //{
        //    // Arrange
        //    var worksImportRequest = OscarContext.WorksImportRequests.First();

        //    var updateWorksImportRequestCommand = new UpdateWorksImportRequestCommand
        //    {
        //        Id = worksImportRequest.Id,
        //        Status = WorksImportRequestStatus.Success
        //    };

        //    // Act
        //    var result = await Mediator.Send(updateWorksImportRequestCommand);
        //    var worksImportRequestUpdated = OscarContext.WorksImportRequests.Find(worksImportRequest.Id);

        //    // Assert
        //    Assert.IsTrue(result.IsSuccess);
        //    Assert.AreEqual(WorksImportRequestStatus.Success, worksImportRequestUpdated.Status);
        //}


        [Ignore]
        public async Task RollbackWorksImportRequest()
        {
            // Arrange
            var worksImportRequest = OscarContext.WorksImportRequests.First();
            worksImportRequest.Status = WorksImportRequestStatus.Rollback;

            await OscarContext.SaveChangesAsync();

            var rollbackWorksImportRequestCommand = new RollbackWorksImportCommand
            {
                Id = worksImportRequest.Id
            };

            // Act
            var result = await Mediator.Send(rollbackWorksImportRequestCommand);
            var worksImportRequestUpdated = OscarContext.WorksImportRequests.Find(worksImportRequest.Id);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(WorksImportRequestStatus.RolledBack, worksImportRequestUpdated.Status);
        }

        [TestMethod]
        public async Task RollbackWorksImportRequest_NotCompleted_ReturnsError()
        {
            // Arrange
            var worksImportRequest = OscarContext.WorksImportRequests.Where(w => w.Status != WorksImportRequestStatus.Success).First();

            await OscarContext.SaveChangesAsync();

            var rollbackWorksImportRequestCommand = new RollbackWorksImportCommand
            {
                Id = worksImportRequest.Id
            };

            // Act
            var result = await Mediator.Send(rollbackWorksImportRequestCommand);

            // Assert
            Assert.IsFalse(result.IsSuccess);
        }

        [TestMethod]
        public async Task DeleteWorksImport()
        {
            // Arrange
            var worksImportRequest = OscarContext.WorksImportRequests.Include(w => w.WorksImports).First();
            var worksImportToDelete = worksImportRequest.WorksImports.First();

            var deleteWorksImportCommand = new DeleteWorksImportCommand
            {
                Id = worksImportToDelete.Id
            };

            // Act
            var result = await Mediator.Send(deleteWorksImportCommand);
            var worksImportRequestUpdated = OscarContext.WorksImportRequests.Include(w => w.WorksImports).First();

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(0, worksImportRequestUpdated.WorksImports.Count);
        }

       

    }
}
