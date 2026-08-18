using Oscar.Core.Entities;
using Oscar.Core.Enums;
using Oscar.Infrastructure.Features.Works.Queries;
using Xunit;
using WorksStatus = Oscar.Core.Enums.WorksStatus;

namespace Oscar.Infrastructure.UnitTests.Works;

public class SearchByTitleQueryShould : UnitTestBase
{
    [Fact]
    public async Task FailValidation_WhenTitleIsNullOrEmpty()
    {
        // Act
        var result = await Mediator.Send(new SearchByTitleQuery { Title = string.Empty });

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task ReturnMatchingWorks_WhenTitleMatches()
    {
        // Arrange
        AddWorksWithTitle("Some Great Movie");
        AddWorksWithTitle("Another Great Movie");
        AddWorksWithTitle("Unrelated Show");
        await OscarContext.SaveChangesAsync();

        // Act
        var result = await Mediator.Send(new SearchByTitleQuery
        {
            Title = "Great Movie",
            SearchType = SearchType.Like
        });

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.TotalRecords);
    }

    [Fact]
    public async Task FilterByDiscriminator()
    {
        // Arrange
        AddWorksWithTitle("Discriminator Test", discriminator: Discriminator.StandAlone);
        AddWorksWithTitle("Discriminator Test", discriminator: Discriminator.Series);
        await OscarContext.SaveChangesAsync();

        // Act
        var result = await Mediator.Send(new SearchByTitleQuery
        {
            Title = "Discriminator Test",
            SearchType = SearchType.Like,
            Discriminator = Discriminator.StandAlone
        });

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value.TotalRecords);
    }

    [Fact]
    public async Task ExcludeUncontrolledWorksByDefault()
    {
        // Arrange
        AddWorksWithTitle("Excluded Title", WorksStatus.Uncontrolled);
        await OscarContext.SaveChangesAsync();

        // Act
        var result = await Mediator.Send(new SearchByTitleQuery
        {
            Title = "Excluded Title",
            SearchType = SearchType.Like
        });

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(0, result.Value.TotalRecords);
    }

    private void AddWorksWithTitle(string title, WorksStatus status = WorksStatus.Active, Discriminator discriminator = Discriminator.StandAlone)
    {
        var works = new Core.Entities.Works
        {
            WorksStatus = status,
            Discriminator = discriminator.ToString(),
            Titles = new List<WorksTitle>
            {
                new() { Title = title, TitleType = TitleType.Main }
            },
            Clients = new List<Client> { new() { ClientName = "TEST_CLIENT" } },
            Catalogues = new List<Catalogue> { new() { Name = "TEST_CATALOGUE" } }
        };

        OscarContext.Works.Add(works);
    }
}
