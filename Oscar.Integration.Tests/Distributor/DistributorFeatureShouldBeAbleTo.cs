using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oscar.Core.DTOs;
using Oscar.Infrastructure.Features.Distributor.Queries;
using System.Linq;
using System.Threading.Tasks;

namespace Oscar.Integration.Tests.Distributor
{
    [TestClass]
    public class DistributorFeatureShouldBeAbleTo : BaseTest
    {
        [TestMethod]
        public async Task GetAllDistributors()
        {
            // Arrange
            // {Use data from base test}

            // Act
            var result = await Mediator.Send(new GetDistributorQuery());

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Value.TotalRecords >= 3);
        }
    }
}
