using Oscar.Infrastructure.Features.Society.Queries;
using Xunit;

namespace Oscar.Infrastructure.UnitTests.Societies;

public class GetAllSocietiesQueryShould : UnitTestBase
{
    [Fact]
    public async Task ReturnSuccess_WhenQueried()
    {
        // Act
        var result = await Mediator.Send(new GetAllSocietiesQuery());

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }
}
