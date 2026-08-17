using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oscar.Core.DTOs;
using Oscar.Infrastructure.Features.Actor.Queries;
using System.Linq;
using System.Threading.Tasks;
using Oscar.Infrastructure.Features.Actor.Commands;

namespace Oscar.Integration.Tests.Actor
{
    [TestClass]
    public class ActorFeatureShouldBeAbleTo : BaseTest
    {
        [TestMethod]
        public async Task GetAllActors()
        {
            // Arrange
            // {Use data from base test}

            // Act
            var result = await Mediator.Send(new GetActorQuery());

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Value.TotalRecords >= 3);
        }

        //[TestMethod]
        //public async Task AddActor()
        //{
        //    // Arrange
        //    // {Use data from base test}

        //    // Act
        //    var result = await Mediator.Send(new AddPersonCommand<Core.Entities.Actor>
        //    {
        //        FirstName = "test",
        //        LastName = "test"
        //    });

        //    var checkResult = await Mediator.Send(new GetActorQuery { Id = result.Value.Id });

        //    // Assert
        //    Assert.IsTrue(result.IsSuccess);
        //    Assert.IsTrue(checkResult.IsSuccess);
        //    Assert.IsTrue(result.Value.FirstName == "test");
        //    Assert.IsTrue(result.Value.LastName == "test");
        //}
    }
}
