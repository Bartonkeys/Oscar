using Oscar.Infrastructure.Features.Channel.Queries;
using Xunit;

namespace Oscar.Infrastructure.UnitTests.Channels;

public class GetAllChannelsQueryShould : UnitTestBase
{
    [Fact]
    public async Task ReturnSuccess_WhenQueried()
    {
        // Act
        var result = await Mediator.Send(new GetAllChannelsQuery());

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }
}
