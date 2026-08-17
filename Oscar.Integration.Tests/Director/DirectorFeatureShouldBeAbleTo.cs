using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oscar.Core.DTOs;
using Oscar.Infrastructure.Features.Director.Queries;
using System.Linq;
using System.Threading.Tasks;

namespace Oscar.Integration.Tests.Director
{
    [TestClass]
    public class DirectorFeatureShouldBeAbleTo : BaseTest
    {
        [TestMethod]
        public async Task GetAllDirectors()
        {
            // Arrange
            // {Use data from base test}

            // Act
            var result = await Mediator.Send(new GetDirectorQuery());

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Value.TotalRecords >= 3);
        }
    }
}
