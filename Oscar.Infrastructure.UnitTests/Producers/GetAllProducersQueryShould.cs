using Oscar.Infrastructure.Features.Producer.Queries;
using Xunit;

namespace Oscar.Infrastructure.UnitTests.Producers;

public class GetAllProducersQueryShould : UnitTestBase
{
    [Fact]
    public async Task ReturnSuccess_WhenQueried()
    {
        // Act
        var result = await Mediator.Send(new GetAllProducersQuery());

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }
}
