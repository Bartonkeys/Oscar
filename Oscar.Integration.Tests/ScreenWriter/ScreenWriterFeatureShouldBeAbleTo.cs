using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oscar.Core.DTOs;
using Oscar.Infrastructure.Features.ScreenWriter.Queries;
using System.Threading.Tasks;

namespace Oscar.Integration.Tests.ScreenWriter
{
    [TestClass]
    public class ScreenWriterFeatureShouldBeAbleTo : BaseTest
    {
        [TestMethod]
        public async Task GetAllScreenWriters()
        {
            // Arrange
            // {Use data from base test}

            // Act
            var result = await Mediator.Send(new GetScreenWriterQuery());

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Value.TotalRecords >= 3);
        }
    }
}
