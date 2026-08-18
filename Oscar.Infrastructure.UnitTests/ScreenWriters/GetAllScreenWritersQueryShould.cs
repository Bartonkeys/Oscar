using Oscar.Infrastructure.Features.ScreenWriter.Queries;
using Xunit;

namespace Oscar.Infrastructure.UnitTests.ScreenWriters;

public class GetAllScreenWritersQueryShould : UnitTestBase
{
    [Fact]
    public async Task ReturnSuccess_WhenQueried()
    {
        // Act
        var result = await Mediator.Send(new GetAllScreenWritersQuery());

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }
}
