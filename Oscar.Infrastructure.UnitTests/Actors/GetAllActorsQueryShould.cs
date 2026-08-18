using Oscar.Infrastructure.Features.Actor.Queries;
using Xunit;

namespace Oscar.Infrastructure.UnitTests.Actors;

public class GetAllActorsQueryShould : UnitTestBase
{
    [Fact]
    public async Task ReturnSuccess_WhenQueried()
    {
        // Act
        var result = await Mediator.Send(new GetAllActorsQuery());

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }
}
