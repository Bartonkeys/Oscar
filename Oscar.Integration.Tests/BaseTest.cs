using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using BartonKeys.Functional;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.DI;
using Oscar.Infrastructure.Features.Common.Contracts;
using Oscar.Infrastructure.Features.Matching.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Distributed;
using Oscar.Core.Providers;

namespace Oscar.Integration.Tests
{
    [TestClass]
    public class BaseTest
    {
        protected OscarContext OscarContext;
        protected IMediator Mediator;
        private readonly ServiceCollection _services = new();

        protected IImporter Importer;
        protected IMatchingService MatchingService;
        IUserProvider UserProvider;
        protected IDistributedCache cache;
        protected ICacheService CacheService;

        [TestInitialize]
        public async Task TestInitialize()
        {
            _services.AddLogging();
            _services.AddDbContext<OscarContext>(options => options.UseInMemoryDatabase("InMemoryDatabase"));
            _services.ConfigureFeatures(Assembly.GetExecutingAssembly());
            _services.AddDistributedMemoryCache();

            _services.AddTransient<IConfiguration>(sp =>
            {
                IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
                configurationBuilder.AddJsonFile("appsettings.test.json");
                return configurationBuilder.Build();
            });

            MockBlobServiceClient();
            MockQueueServiceClient();
            MockImporter();
            MockExporter();
            MockUserProvider();

            var servicesProvider = _services.BuildServiceProvider();

            Mediator = servicesProvider.GetRequiredService<IMediator>();
            OscarContext = servicesProvider.GetRequiredService<OscarContext>();

            Importer = servicesProvider.GetRequiredService<IImporter>();
            UserProvider = servicesProvider.GetRequiredService<IUserProvider>();
            
            MatchingService = servicesProvider.GetRequiredService<IMatchingService>();
            cache = servicesProvider.GetRequiredService<IDistributedCache>();
            CacheService = servicesProvider.GetRequiredService<ICacheService>();

            await SetUpTestData();
            await SetUpMatchingTestData();
        }

        public static Mock<IFormFile> MockFile(string filename)
        {
            var fileMock = new Mock<IFormFile>();
            var content = "Gabba,Gabba,Hey";
            var fileName = filename;
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(ms.Length);
            return fileMock;
        }

        private void MockBlobServiceClient()
        {
            var blobContentInfo = BlobsModelFactory.BlobContentInfo(new ETag(), new DateTimeOffset(), new Byte[1], "", "", "", 10);
            var mockBlobResponse = new Mock<Response>();

            var mockBlobContainerClient = new Mock<BlobContainerClient>();
            mockBlobContainerClient.Setup(m => m.UploadBlobAsync(It.IsAny<string>(), It.IsAny<Stream>(), CancellationToken.None)).ReturnsAsync(Azure.Response.FromValue(blobContentInfo, mockBlobResponse.Object));

            var mockBlobServiceClient = new Mock<BlobServiceClient>();
            mockBlobServiceClient.Setup(m => m.GetBlobContainerClient(It.IsAny<string>())).Returns(mockBlobContainerClient.Object);

            _services.AddSingleton(mockBlobServiceClient.Object);
        }

        private void MockQueueServiceClient()
        {
            var sendReceipt = QueuesModelFactory.SendReceipt("", new DateTimeOffset(), new DateTimeOffset(), "", new DateTimeOffset());
            var mockQueueResponse = new Mock<Response>();

            var mockQueueClient = new Mock<QueueClient>();
            mockQueueClient.Setup(m => m.SendMessageAsync(It.IsAny<string>(), CancellationToken.None)).ReturnsAsync(Azure.Response.FromValue(sendReceipt, mockQueueResponse.Object));

            var mockQueueServiceClient = new Mock<QueueServiceClient>();
            mockQueueServiceClient.Setup(m => m.GetQueueClient(It.IsAny<string>())).Returns(mockQueueClient.Object);

            _services.AddSingleton(mockQueueServiceClient.Object);
        }

        private void MockImporter()
        {
            var importerResult1 = new List<MatchTemplateDto>
            {
                new MatchTemplateDto()
                {
                    Line = Guid.NewGuid().ToString(),
                    Title1 = $"Beyond the law",
                    SeasonNo = "1",
                    EpisodeNo = "2",
                    Duration = "90",
                    ShareAvailable = "Y",
                    ProductionCountry = new string[3] { "GB", "France", "Spain" },
                    Director1 = "Larry Ferguson",
                    ProductionType = "Short",
                    Channel = "BBC",
                    BroadcastDate = "06/14/2002"
                }
            };


            var importerResult2 = new List<MatchTemplateDto>
            {
                new MatchTemplateDto()
                {
                    Line = Guid.NewGuid().ToString(),
                    Title1 = $"Jaws the Revenge",
                    SeasonNo = "1",
                    EpisodeNo = "1",
                    Duration = "15",
                    ShareAvailable = "Y",
                    ProductionCountry = new string[3] { "GB", "France", "Spain" },
                    Director1 = "Liam Mailey",
                    ProductionType = "Short",
                    Channel = "BBC",
                    BroadcastDate = "06/14/2002"
                }
            };


            var importer = new Mock<Oscar.Infrastructure.Features.Common.Contracts.IImporter>();
            importer.Setup(m => m.ImportMatchCsvAsList("TEST_REF_01.csv")).Returns(Result.Ok(importerResult1));
            importer.Setup(m => m.ImportMatchCsvAsList("TEST_REF_02.csv")).Returns(Result.Ok(importerResult2));
            importer.Setup(m => m.ImportMatchCsvAsList("BK_oscar_match_test.csv")).Returns(Result.Ok(importerResult1));


            var separator = Path.DirectorySeparatorChar;
            var filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.Replace($"bin{separator}Debug{separator}net6.0", $"{separator}TestFiles{separator}SampleFile.csv"));
            var bytes = File.ReadAllBytes(filePath);
            importer.Setup(m => m.ImportMatchBlobAsBytes(It.IsAny<string>())).Returns(Result.Ok(bytes));
            
            _services.AddSingleton(importer.Object);

  
        }

        private void MockExporter()
        {
            var exporter = new Mock<Oscar.Infrastructure.Features.Common.Contracts.IExporter>();
            exporter.Setup(m => m.ExportListAsCsv(It.IsAny<IEnumerable<MatchTemplateResultsDto>>(), It.IsAny<string>())).Returns(Result.Ok());
            _services.AddSingleton(exporter.Object);
        }

        private void MockUserProvider()
        {
            var userProvider = new Mock<IUserProvider>();
            userProvider.Setup(u => u.GetUserName()).Returns("TestUser");
            _services.AddSingleton(userProvider.Object);
        }

        private async Task SetUpMatchingTestData()
        {

            var client1 = new Client()
            {
                ClientName = "Test client one",
                Status = Core.Enums.Status.Active_In_Term,
                Contract = new Contract()
                {
                    EndDate = DateTime.Parse("2030-01-01"),
                    CurrentStartDate = DateTime.Parse("2000-01-01"),
                }
            };

            CountryGroup territoryGroup1 = new() {
                GroupName = "Test T GROUP"
            };

            territoryGroup1.Countries = new List<Country>
            {
                new Country() { Id = 333, Code = "t1", Name = "t1" }
            };

            var standAlone1 = new Core.Entities.StandAlone
            {
                ProductionYear = 1998,
                DurationMinutes = 90,
                Titles = new List<WorksTitle>()
            };
            standAlone1.Titles.Add(new WorksTitle { Title = "Beyond the Law" });
            standAlone1.Titles.Add(new WorksTitle { Title = "Fixing the Shadow" });
            standAlone1.Directors = new List<Core.Entities.Director>();
            standAlone1.Directors.Add(new Core.Entities.Director { FirstName = "Larry", LastName = "Ferguson" });
            standAlone1.Rights = new List<Right>();
            standAlone1.Rights.Add(new Right()
            {
                Client = client1,
                StartOfRight = DateTime.Parse("1999-01-01"),
                EndOfRight = DateTime.Parse("2004-01-01"),
                Notations = "Test",
                Type = new RightsType { Id = 1, Name ="Test" }
            });
            standAlone1.Countries = new List<Country>();
            standAlone1.Countries.Add(new Country() {
                Id = 666,
                Name = "GB",
                Description = "COUNTRY_DESCRIPTION",
                Code = "GB",
                Code3A = "GB"
            });
            standAlone1.Clients = new List<Client>();
            standAlone1.Clients.Add(client1);
            OscarContext.StandAlones.Add(standAlone1);
            await OscarContext.SaveChangesAsync();

            var matchRequest1 = new MatchRequest()
            {
                Reference = "TEST_REF_01",
                Status = Core.Enums.MatchRequestStatus.Pending,
                Rules = MatchRules.TitleCheckLevel1
                    | MatchRules.TitleCheckLevel2
                    | MatchRules.TitleCheckLevel3
                    //| MatchRules.Territory
                    | MatchRules.RightsYears
                    | MatchRules.Director
                    | MatchRules.Duration
                    //| MatchRules.Territory
                    | MatchRules.ProductionYear
                    | MatchRules.RightsType
                    | MatchRules.ProductionCountry,
                RequestedBy = "TestUser1",
                IgnoreCharactersFollowing = ":",
                TerritoryId = 1,
                RightsFromYear = 1999,
                RightsToYear = 2004,
                RightsTypeId = 1,
                ClientId = client1.Id
            };
            OscarContext.MatchRequests.Add(matchRequest1);
            await OscarContext.SaveChangesAsync();


            var client2 = new Client()
            {
                ClientName = "Test client two",
                Status = Core.Enums.Status.Active_In_Term,
                Contract = new Contract()
                {
                    EndDate = DateTime.Parse("2030-01-01"),
                    CurrentStartDate = DateTime.Parse("2000-01-01"),
                }
            };

            CountryGroup territoryGroup2 = new CountryGroup()
            {
                GroupName = "Test T GROUP two",
            };

            territoryGroup2.Countries = new List<Country>();
            territoryGroup2.Countries.Add(new Country() { Id = 444, Code = "IRL", Name = "Test" });

            var standAlone2 = new Oscar.Core.Entities.StandAlone();
            standAlone2.ProductionYear = 1970;
            standAlone2.DurationMinutes = 200;
            standAlone2.Titles = new List<WorksTitle>();
            standAlone2.Titles.Add(new WorksTitle { Title = "Jaws the Revenge" });
            standAlone2.Directors = new List<Core.Entities.Director>();
            standAlone2.Directors.Add(new Core.Entities.Director { FirstName = "Joey", LastName = "Ramone" });
            standAlone2.Rights = new List<Right>();
            standAlone2.Rights.Add(new Right()
            {
                Client = client2,
                StartOfRight = DateTime.Parse("2000-01-01"),
                EndOfRight = DateTime.Parse("2030-01-01"),
                Notations = "Test",
                Type = new RightsType { Id = 2, Name = "Test" }
            });
            standAlone2.Countries = new List<Country>();
            standAlone2.Countries.Add(new Country() {
                Id = 555,
                Name = "IRELAND",
                Description = "COUNTRY_DESCRIPTION",
                Code = "IRL",
                Code3A = "IRL"
            });
            standAlone2.Clients = new List<Client>();
            standAlone2.Clients.Add(client2);
            OscarContext.StandAlones.Add(standAlone2);
            await OscarContext.SaveChangesAsync();

            var matchRequest2 = new MatchRequest()
            {
                Reference = "TEST_REF_02",
                Status = MatchRequestStatus.Pending,
                Rules = MatchRules.TitleCheckLevel1 
                    | MatchRules.TitleCheckLevel2 
                    | MatchRules.TitleCheckLevel3
                   // | MatchRules.Territory 
                    | MatchRules.RightsYears 
                    | MatchRules.Director 
                    | MatchRules.Duration 
                   // | MatchRules.Territory
                    | MatchRules.ProductionYear
                    | MatchRules.RightsType
                    | MatchRules.ProductionCountry,
                RequestedBy = "TestUser2",
                IgnoreCharactersFollowing = ":",
                TerritoryId = 1,
                RightsFromYear = 1999,
                RightsToYear = 2004,
                ClientId = client2.Id,
                RightsTypeId = 1000
            };
            OscarContext.MatchRequests.Add(matchRequest2);
            await OscarContext.SaveChangesAsync();


        }

        private async Task SetUpTestData()
        {

            for (int i = 0; i < 2; i++)
            {
                var genre = new Oscar.Core.Entities.Genre();
                genre.Name = $"Genre{i}";
                genre.CreationDate = DateTime.Now;
                OscarContext.Genres.Add(genre);
            }


            for (int i = 0; i < 3; i++)
            {
                var client = new Oscar.Core.Entities.Client();
                client.Status = Core.Enums.Status.Active_Consolidated;
                client.ClientGrade = Core.Enums.ClientGrade.Platinum;
                client.ClientType = Core.Enums.ClientType.Broadcaster;
                client.ClientReference = i;
                client.ClientName= $"NAME{i}";
                client.IMaestroClientCode = $"IMCC{i}";
                client.Email = $"test{i}@test.com";
                client.GeneralNotes = $"NOTE{i}";
                client.CreationDate = DateTime.Now;
                client.Addresses = new List<Address>();
                client.Addresses.Add(new Address() { AddressLine1 = $"{i} Some Street", AddressLine2="Somewhere", PostZipCode=$"BT77 0T{i}", IsCurrent = false });
                client.Addresses.Add(new Address() { AddressLine1 = $"{i} Another Street", AddressLine2 = "Somewhere else", PostZipCode = $"BT66 0T{i}", IsCurrent = true });
                client.Catalogues = new List<Oscar.Core.Entities.Catalogue>{new Core.Entities.Catalogue{Name = $"NAME{i}" } };
                client.Catalogues.Add(new Oscar.Core.Entities.Catalogue { Name = "Test", CreationDate = DateTime.Now, IMaestroClientCode = "Test"});
                client.Rights = new List<Right>
                {
                    new Right
                    {
                        StartOfRight = DateTime.Now, EndOfRight = DateTime.Now, StartOfValidity = DateTime.Now,
                        EndOfValidity = DateTime.Now
                    }
                };

                OscarContext.Clients.Add(client);

            }

            for (int i = 0; i < 2; i++)
            {
                var contact = new Oscar.Core.Entities.Contact();
                contact.FirstName = $"FIRST{i}";
                contact.LastName = $"LAST{i}";
                contact.CreationDate = DateTime.Now;
                OscarContext.Contacts.Add(contact);
            }

            for (int i = 0; i < 2; i++)
            {
                var customServiceManager = new Oscar.Core.Entities.CustomerServiceManager
                {
                    IsActive = true,
                    Operator = new Operator
                    {
                        FullName = $"FULL{i}"
                    }
                };
                customServiceManager.CreationDate = DateTime.Now;
                OscarContext.CustomServiceManagers.Add(customServiceManager);
            }

            for (int i = 0; i < 2; i++)
            {
                var series = new Oscar.Core.Entities.Series();
                series.AgicoaWorksReference = $"ADN{i}";
                series.CavcoCode = $"CCC{i}";
                series.CreationDate = DateTime.Now;
                series.FirstBroadcastYear = 1999;
                series.GeneralNotes = $"NOTE{i}";
                series.GenreId = 1;
                series.IMaestroWorkCode = $"IMWC{i}";
                series.Isan = $"ISAN{i}";
                series.ProductionYear = 1998;
                series.DurationMinutes = 60;
                series.Number = 1000 + i;
                series.WorksStatus = Core.Enums.WorksStatus.Active;
                OscarContext.Series.Add(series);
            }

            for (int i = 0; i < 2; i++)
            {
                var standAlone = new Oscar.Core.Entities.StandAlone();
                standAlone.AgicoaWorksReference = $"ADN{i}";
                standAlone.CavcoCode = $"CCC{i}";
                standAlone.CreationDate = DateTime.Now;
                standAlone.FirstBroadcastYear = 1999;
                standAlone.GeneralNotes = $"NOTE{i}";
                standAlone.GenreId = 1;
                standAlone.IMaestroWorkCode = $"IMWC{i}";
                standAlone.Isan = $"ISAN{i}";
                standAlone.ProductionYear = 1998;
                standAlone.DurationMinutes = 60;
                standAlone.Number = 1000 + i;
                standAlone.WorksStatus = Core.Enums.WorksStatus.Active;
                OscarContext.StandAlones.Add(standAlone);
            }

            for (int i = 0; i < 2; i++)
            {
                var season = new Oscar.Core.Entities.Season();
                season.AgicoaWorksReference = $"ADN{i}";
                season.CavcoCode = $"CCC{i}";
                season.CreationDate = DateTime.Now;
                season.FirstBroadcastYear = 1999;
                season.GeneralNotes = $"NOTE{i}";
                season.GenreId = 1;
                season.IMaestroWorkCode = $"IMWC{i}";
                season.Isan = $"ISAN{i}";
                season.ProductionYear = 1998;
                season.DurationMinutes = 60;
                season.Number = 1000 + i;
                season.SeriesId = 1;
                season.WorksStatus = Core.Enums.WorksStatus.Active;
                OscarContext.Seasons.Add(season);
            }

            for (int i = 0; i < 2; i++)
            {
                var episode = new Oscar.Core.Entities.Episode();
                episode.AgicoaWorksReference = $"ADN{i}";
                episode.CavcoCode = $"CCC{i}";
                episode.CreationDate = DateTime.Now;
                episode.FirstBroadcastYear = 1999;
                episode.GeneralNotes = $"NOTE{i}";
                episode.GenreId = 1;
                episode.IMaestroWorkCode = $"IMWC{i}";
                episode.Isan = $"ISAN{i}";
                episode.ProductionYear = 1998;
                episode.DurationMinutes = 60;
                episode.Number = 1000 + i;
                episode.SeasonId = 1;
                episode.WorksStatus = Core.Enums.WorksStatus.Active;
                OscarContext.Episodes.Add(episode);
            }

            for (int i = 0; i < 3; i++)
            {
                var country = new Oscar.Core.Entities.Country();
                country.Id = i;
                country.Name = $"COUNTRY_NAME_{i}";
                country.Description = $"COUNTRY_DESCRIPTION_{i}";
                country.Code = $"{i}";
                country.Code3A = $"{i}";
        
                OscarContext.Country.Add(country);
            }

            var reportEntityJoin = new Oscar.Core.Entities.ReportEntityJoin();
            reportEntityJoin.BaseEntityName = "Clients";
            reportEntityJoin.JoinEntityName = "Works";
            reportEntityJoin.JoinExpresssion = " inner join ClientWorks on ClientWorks.ClientsId = Clients.Id inner join Works on ClientWorks.WorksId = Works.Id ";
            OscarContext.ReportentityJoins.Add(reportEntityJoin);


            var report = new Oscar.Core.Entities.Report();
            report.ReportName = "REPORT_NAME";
            report.BaseEntityName = "Clients";
            report.ReportFields = new ReportField[]
            {
                new ReportField { BaseEntityName = "Clients", ReportFieldName = "Id"},
                new ReportField { BaseEntityName = "Works", ReportFieldName = "WorksStatus"}
            };

            OscarContext.Reports.Add(report);
            
            for (int i = 0; i < 3; i++)
            {
                var genre = new Oscar.Core.Entities.Genre();
                genre.Name = $"GENRE_NAME_{i}";
                genre.Description = $"GENRE_DESCRIPTION_{i}";
         
                OscarContext.Genres.Add(genre);
            }

            for (int i = 0; i < 3; i++)
            {
                var language = new Oscar.Core.Entities.Language();
                language.Name = $"LANGUAGE_NAME_{i}";
                language.Description = $"LANGUAGE_DESCRIPTION_{i}";

                OscarContext.Languages.Add(language);
            }

            for (int i = 0; i < 3; i++)
            {
                var actor = new Oscar.Core.Entities.Actor();
                actor.FirstName = $"ACTOR_FIRST_NAME_{i}";
                actor.LastName = $"ACTOR_LAST_NAME{i}";

                OscarContext.Actors.Add(actor);
            }

            for (int i = 0; i < 3; i++)
            {
                var director = new Oscar.Core.Entities.Director();
                director.FirstName = $"DIRECTOR_FIRST_NAME_{i}";
                director.LastName = $"DIRECTOR_LAST_NAME{i}";

                OscarContext.Directors.Add(director);
            }

            for (int i = 0; i < 3; i++)
            {
                var catalogue = new Oscar.Core.Entities.Catalogue();
                catalogue.Name = $"CATALOGUE_NAME_{i}";
                OscarContext.Catalogues.Add(catalogue);
            }

            for (int i = 0; i < 3; i++)
            {
                var distributor = new Oscar.Core.Entities.Distributor();
                distributor.FirstName = $"DISTRIBUTOR_FIRST_NAME_{i}";
                distributor.LastName = $"DISTRIBUTOR_LAST_NAME_{i}";

                OscarContext.Distributors.Add(distributor);
            }

            for (int i = 0; i < 3; i++)
            {
                var screenWriter = new Oscar.Core.Entities.ScreenWriter();
                screenWriter.FirstName = $"SCREENWRITER_FIRST_NAME_{i}";
                screenWriter.LastName = $"SCREENWRITER_LAST_NAME_{i}";

                OscarContext.ScreenWriters.Add(screenWriter);
            }

            await OscarContext.SaveChangesAsync();

            for (int i = 0; i < 3; i++)
            {
                var worksImportRequest = new Oscar.Core.Entities.WorksImportRequest();
                var client = OscarContext.Clients.First();
                OscarContext.WorksImportRequests.Add(worksImportRequest);
               // worksImportRequest.ClientId = client.Id;
                worksImportRequest.CatalogueId = client.Catalogues.First().Id;
                worksImportRequest.Reference = "TestRef";
                worksImportRequest.Status = WorksImportRequestStatus.Pending;
                worksImportRequest.CreationDate = DateTime.Now;
                worksImportRequest.RequestedBy = "TestUser";

                var worksImport = new Oscar.Core.Entities.WorksImport();
                worksImport.WorksType = "Stand Alone";
                worksImport.SASeriesNumber = "1";
                worksImport.Title = "Test title";
                worksImport.ProductionYear = "1999";
                worksImport.Duration = "55";
                worksImport.DirectorFirstName = "James";
                worksImport.DirectorLastName = "Smith";
                worksImport.ProductionCompany1 = "Test Company";
                worksImport.ProductionCountry1 = "UK";

                worksImportRequest.WorksImports = new List<Oscar.Core.Entities.WorksImport>();
                worksImportRequest.WorksImports.Add(worksImport);
            }


            await OscarContext.SaveChangesAsync();
 
        }

        [TestCleanup]
        public void Cleanup()
        {
            OscarContext.Database.EnsureDeleted();
        }
    }

}