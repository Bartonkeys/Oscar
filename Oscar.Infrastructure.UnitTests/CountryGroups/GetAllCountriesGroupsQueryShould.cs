using Oscar.Infrastructure.Features.Country.Queries;
using Xunit;

namespace Oscar.Infrastructure.UnitTests.CountryGroups;

public class GetAllCountriesGroupsQueryShould : UnitTestBase
{
    [Fact]
    public async Task ReturnSuccess_WhenQueried()
    {
        // Act
        var result = await Mediator.Send(new GetAllCountriesGroupsQuery());

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }
}
