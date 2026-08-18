using Oscar.Infrastructure.Features.Country.Queries;
using Xunit;

namespace Oscar.Infrastructure.UnitTests.Countries;

public class GetAllCountriesQueryShould : UnitTestBase
{
    [Fact]
    public async Task ReturnSuccess_WhenQueried()
    {
        // Act
        var result = await Mediator.Send(new GetAllCountriesQuery());

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }
}
