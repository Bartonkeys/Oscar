using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oscar.Core.Entities;
using Oscar.Infrastructure.Features.Rights.Queries;

namespace Oscar.Integration.Tests.Rights;

[TestClass]
public class RightsFeatureShouldBeAbleTo : BaseTest
{
    [TestMethod]
    public async Task GetRightsByClientId()
    {
        // Arrange
        var client = OscarContext.Clients.Single(c => c.ClientName == "TEST CLIENT ONE");
        client.Rights = new List<Right>
        {
            new Right
            {
                StartOfRight = DateTime.Now,
                StartOfValidity = DateTime.Now,
                EndOfRight = DateTime.Now,
                EndOfValidity = DateTime.Now
            }
        };
        await OscarContext.SaveChangesAsync();

        // Act
        var result = await Mediator.Send(new GetRightsByClientIdQuery
        {
            ClientId = client.Id
        });

        // Assert
        Assert.IsTrue(result.IsSuccess);
        //Assert.IsTrue(result.Value.Any());
    }
}