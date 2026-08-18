using Oscar.Infrastructure.Features.Director.Queries;
using Xunit;

namespace Oscar.Infrastructure.UnitTests.Directors;

public class GetAllDirectorsQueryShould : UnitTestBase
{
    [Fact]
    public async Task ReturnSuccess_WhenQueried()
    {
        // Act
        var result = await Mediator.Send(new GetAllDirectorsQuery());

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }
}
