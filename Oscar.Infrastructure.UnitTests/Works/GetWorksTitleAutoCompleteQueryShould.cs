using Oscar.Core.Entities;
using Oscar.Core.Enums;
using Oscar.Infrastructure.Features.Works.Queries;
using Xunit;
using WorksStatus = Oscar.Core.Enums.WorksStatus;

namespace Oscar.Infrastructure.UnitTests.Works;

public class GetWorksTitleAutoCompleteQueryShould : UnitTestBase
{
    [Fact]
    public async Task ReturnMatchingTitles_WhenSearchTypeIsStartsWith()
    {
        // Arrange
        AddWorksWithTitle("Apollo 13");
        AddWorksWithTitle("Apollo 18");
        AddWorksWithTitle("Not Matching");
        await OscarContext.SaveChangesAsync();

        // Act
        var result = await Mediator.Send(new GetWorksTitleAutoCompleteQuery
        {
            Title = "Apollo",
            SearchType = SearchType.StartsWith
        });

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count());
    }

    [Fact]
    public async Task ExcludeUncontrolledWorks()
    {
        // Arrange
        AddWorksWithTitle("Uncontrolled Title", WorksStatus.Uncontrolled);
        await OscarContext.SaveChangesAsync();

        // Act
        var result = await Mediator.Send(new GetWorksTitleAutoCompleteQuery
        {
            Title = "Uncontrolled",
            SearchType = SearchType.StartsWith
        });

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task RespectMaxCount()
    {
        // Arrange
        for (var i = 0; i < 5; i++)
        {
            AddWorksWithTitle($"Duplicate Title {i}");
        }
        await OscarContext.SaveChangesAsync();

        // Act
        var result = await Mediator.Send(new GetWorksTitleAutoCompleteQuery
        {
            Title = "Duplicate",
            SearchType = SearchType.StartsWith,
            MaxCount = 3
        });

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value.Count() <= 3);
    }

    private void AddWorksWithTitle(string title, WorksStatus status = WorksStatus.Active)
    {
        var works = new Core.Entities.Works
        {
            WorksStatus = status,
            Discriminator = Discriminator.StandAlone.ToString(),
            Titles = new List<WorksTitle>
            {
                new() { Title = title, TitleType = TitleType.Main }
            }
        };

        OscarContext.Works.Add(works);
    }
}
