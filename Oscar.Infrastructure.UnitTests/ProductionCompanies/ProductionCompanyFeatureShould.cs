using Oscar.Core.DTOs;
using Oscar.Infrastructure.Features.ProductionCompany.Commands;
using Oscar.Infrastructure.Features.ProductionCompany.Queries;
using Xunit;

namespace Oscar.Infrastructure.UnitTests.ProductionCompanies;

public class ProductionCompanyFeatureShould : UnitTestBase
{
    [Fact]
    public async Task ReturnSuccess_WhenQueriedForAll()
    {
        // Act
        var result = await Mediator.Send(new GetAllCompaniesQuery());

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }

    [Fact]
    public async Task AddCompany()
    {
        // Arrange
        var companyAddDto = new CompanyAddDto
        {
            Name = "Test Production Company"
        };

        // Act
        var result = await Mediator.Send(new AddCompanyCommand { CompanyAddDto = companyAddDto });

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value.Id > 0);
    }
}
