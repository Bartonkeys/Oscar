using Oscar.Core.Entities;
using Oscar.Core.Enums;
using Oscar.Infrastructure.Features.Works.Queries;
using Xunit;
using WorksStatus = Oscar.Core.Enums.WorksStatus;

namespace Oscar.Infrastructure.UnitTests.Works;

public class GetWorksTitleQueryShould : UnitTestBase
{
    [Fact]
    public async Task ReturnMainTitle_WhenWorksIdExists()
    {
        // Arrange
        var works = new Core.Entities.Works
        {
            WorksStatus = WorksStatus.Active,
            Discriminator = Discriminator.StandAlone.ToString(),
            Titles = new List<WorksTitle>
            {
                new() { Title = "Main Title", TitleType = TitleType.Main }
            }
        };

        OscarContext.Works.Add(works);
        await OscarContext.SaveChangesAsync();

        // Act
        var result = await Mediator.Send(new GetWorksTitleQuery { Id = works.Id });

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("MAIN TITLE", result.Value!.Title);
    }

    [Fact]
    public async Task ReturnNull_WhenWorksIdDoesNotExist()
    {
        // Act
        var result = await Mediator.Send(new GetWorksTitleQuery { Id = -1 });

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Null(result.Value);
    }
}
