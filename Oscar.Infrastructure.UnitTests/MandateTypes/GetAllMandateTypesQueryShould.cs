using Oscar.Infrastructure.Features.MandateTypes.Queries;
using Xunit;

namespace Oscar.Infrastructure.UnitTests.MandateTypes;

public class GetAllMandateTypesQueryShould : UnitTestBase
{
    [Fact]
    public async Task ReturnSuccess_WhenQueried()
    {
        // Act
        var result = await Mediator.Send(new GetAllMandateTypesQuery());

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }
}
