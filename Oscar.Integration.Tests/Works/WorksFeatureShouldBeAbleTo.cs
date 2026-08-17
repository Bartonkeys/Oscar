using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oscar.Core.DTOs;
using Oscar.Infrastructure.Features.Works.Commands;
using Oscar.Infrastructure.Features.Works.Queries;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Oscar.Integration.Tests.Works
{
    [TestClass]
    public class WorksFeatureShouldBeAbleTo : BaseTest
    {
        [TestMethod]
        public async Task GetAllWorks()
        {
            // Arrange
            for (var i = 0; i < 10; i++)
            {
                var worksDto = new WorksDto
                {
                    DurationMinutes = 100,
                    ProductionYear = 1901,
                    FirstBroadcastYear = 1901,
                    IMaestroWorkCode = "MB_TEST",
                    AgicoaWorksReference = "MB_TEST",
                    Isan = "MB_TEST",
                    CavcoCode = "MB_TEST",
                    GeneralNotes = "MB_TEST"
                };

                var _ = await Mediator.Send(new AddWorksCommand { WorksDto = worksDto });
            }

            // Act
            var result = await Mediator.Send(new GetWorksQuery());

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Value.TotalRecords >= 10);
        }

        [TestMethod]
        public async Task GetWorksById()
        {
            // Arrange
            var works = OscarContext.Works.Last();

            // Act
            var result = await Mediator.Send(new GetWorksByIdQuery { Id = works.Id });

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(works.Id, result.Value.Id);
        }


        [TestMethod]
        public async Task AddWorks()
        {
            // Arrange
            var worksDto = new WorksDto
            {
                DurationMinutes = 100,
                ProductionYear = 1901,
                FirstBroadcastYear = 1901,
                IMaestroWorkCode = "MB_TEST",
                AgicoaWorksReference = "MB_TEST",
                Isan = "MB_TEST",
                CavcoCode = "MB_TEST",
                GeneralNotes = "MB_TEST"
            };

            // Act
            var result = await Mediator.Send(new AddWorksCommand { WorksDto = worksDto });

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Value.Id > 0);
        }


        [TestMethod]
        public async Task DeleteWorks()
        {
            // Arrange
            var worksDto = new WorksDto
            {
                DurationMinutes = 100,
                ProductionYear = 1901,
                FirstBroadcastYear = 1901,
                IMaestroWorkCode = "MB_TEST",
                AgicoaWorksReference = "MB_TEST",
                Isan = "MB_TEST",
                CavcoCode = "MB_TEST",
                GeneralNotes = "MB_TEST"
            };

            // Act
            var result = await Mediator.Send(new AddWorksCommand { WorksDto = worksDto });

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Value.Id > 0);

            // Act
            var resultDelete = await Mediator.Send(new DeleteWorksCommand { WorksDto = result.Value });

            // Assert
            Assert.IsTrue(resultDelete.IsSuccess);
            Assert.IsTrue(resultDelete.Value.Id > 0);
        }

        [TestMethod]
        public async Task SearchByTitle()
        {
            // Arrange
            for (var i = 0; i < 10; i++)
            {
                var worksDto = new WorksDto
                {
                    DurationMinutes = 100,
                    ProductionYear = 1901,
                    FirstBroadcastYear = 1901,
                    IMaestroWorkCode = "MB_TEST",
                    AgicoaWorksReference = "MB_TEST",
                    Isan = "MB_TEST",
                    CavcoCode = "MB_TEST",
                    GeneralNotes = "MB_TEST",
                    Titles = new List<WorksTitleDto>
                    {
                        new WorksTitleDto{Title = "testTitle"},
                        new WorksTitleDto{Title = "testTitle"}
                    }
                };

                var _ = await Mediator.Send(new AddWorksCommand { WorksDto = worksDto });
            }

            // Act
            var result = await Mediator.Send(new SearchByTitleQuery
            {
                Title = "testTitle"
            });

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Value.TotalRecords >= 10);
        }

    }
}
