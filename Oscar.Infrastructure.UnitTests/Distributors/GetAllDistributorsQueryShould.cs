using Oscar.Infrastructure.Features.Distributor.Queries;
using Xunit;

namespace Oscar.Infrastructure.UnitTests.Distributors;

public class GetAllDistributorsQueryShould : UnitTestBase
{
    [Fact]
    public async Task ReturnSuccess_WhenQueried()
    {
        // Act
        var result = await Mediator.Send(new GetAllDistributorsQuery());

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }
}
