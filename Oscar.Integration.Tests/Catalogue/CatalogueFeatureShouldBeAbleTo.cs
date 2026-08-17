using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oscar.Core.DTOs;
using Oscar.Infrastructure.Features.Catalogue.Queries;
using System.Linq;
using System.Threading.Tasks;

namespace Oscar.Integration.Tests.Catalogue
{
    [TestClass]
    public class CatalogueFeatureShouldBeAbleTo : BaseTest
    {
        [TestMethod]
        public async Task GetAllCatalogues()
        {
            // Arrange
            // {Use data from base test}

            // Act
            var result = await Mediator.Send(new GetCatalogueQuery());

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Value.TotalRecords >= 3);
        }
    }
}
