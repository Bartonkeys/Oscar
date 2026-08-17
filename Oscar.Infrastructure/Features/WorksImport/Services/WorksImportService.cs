using AutoMapper;
using BartonKeys.Functional;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using System.Globalization;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Oscar.Infrastructure.Features.Common;
using Oscar.Infrastructure.Features.Works.Queries;
using Oscar.Infrastructure.Features.WorksImport.Commands;
using WorksStatus = Oscar.Core.Enums.WorksStatus;
using System.Xml.Serialization;

namespace Oscar.Infrastructure.Features.WorksImport.Services
{
    public class WorksImportService : IWorksImportService
    {
        private readonly OscarContext OscarContext;
        private readonly IMapper Mapper;
        private readonly ILogger<WorksImportService> Logger;
        private readonly IMediator _mediator;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public WorksImportService(OscarContext oscarContext,
            IMapper mapper,
            ILogger<WorksImportService> logger, IMediator mediator, IServiceScopeFactory serviceScopeFactory)
        {
            OscarContext = oscarContext;
            Mapper = mapper;
            Logger = logger;
            _mediator = mediator;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public List<WorksImportDto> WorksImportDtoListFromFile(IFormFile? formFile, bool isAgicoa)
        {
            if (formFile == null)
            {
                throw new ArgumentNullException(nameof(formFile));
            }

            List<WorksImportDto> results;

            if (!isAgicoa)
            {
                results = WorksImportDtoListFromCsvFile(formFile);
            }
            else
            {
                results = WorksImportDtoListFromXMLFile(formFile);
            }

            return results;
        }

        public List<WorksImportDto> WorksImportDtoListFromCsvFile(IFormFile? formFile)
        {
            using var reader = new StreamReader(formFile.OpenReadStream());
            using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);
            csvReader.Context.RegisterClassMap<WorksImportDtoMap>();
            var results = csvReader.GetRecords<WorksImportDto>().ToList();
            return results;
        }

        public List<WorksImportDto> WorksImportDtoListFromXMLFile(IFormFile? formFile)
        {
            using var stream = formFile.OpenReadStream();
            var serializer = new XmlSerializer(typeof(WorksImportAgicoaDto));
            var xmlResults = (WorksImportAgicoaDto)serializer.Deserialize(stream);
            var results = WorksImportMapper.Map(xmlResults);

            return results;
        }

        public List<EpisodeImportDto> EpisodeImportDtoListFromFile(IFormFile? formFile)
        {
            using var reader = new StreamReader(formFile.OpenReadStream());
            using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);
            csvReader.Context.RegisterClassMap<EpisodeImportDtoMap>();
            var results = csvReader.GetRecords<EpisodeImportDto>().ToList();
            return results;
        }

        public async Task<Result> WriteWorksRecords(WorksImportRequest worksImportRequest, Client client, Oscar.Core.Entities.Catalogue? catalogue)
        {
            try
            {
                if (worksImportRequest == null || worksImportRequest.WorksImports == null) return Result.Ok();

                var standAloneWorksList = worksImportRequest.WorksImports.Where(w => w.WorksType != null && w.WorksType.ToLower() == "stand alone");
                foreach (var import in standAloneWorksList)
                    await ProcessStandAlone(import, worksImportRequest, client, catalogue);

                var seriesWorksList = worksImportRequest.WorksImports.Where(w => w.WorksType != null && w.WorksType.ToLower() == "series");
                foreach (var import in seriesWorksList)
                    await ProcessSeries(import, worksImportRequest, client, catalogue);

                return Result.Ok();
            }
            catch (Exception ex)
            {
                var worksList = OscarContext.Works.Where(w => w.WorksImportRequest != null && w.WorksImportRequest.Id == worksImportRequest.Id);
                OscarContext.Works.RemoveRange(worksList);
                await OscarContext.SaveChangesAsync();
                Logger.LogError((int)WorksImportRequestFeatureEvent.AddBadRequest, ex, ex.Message);
                return Result.Fail(ex.Message);
            }

        }

        private async Task ProcessStandAlone(Core.Entities.WorksImport import, WorksImportRequest worksImportRequest, Client client, Core.Entities.Catalogue catalogue)
        {
            var result = await _mediator.Send(new SetWorksEntityCommand
            {
                worksImportRequest = worksImportRequest,
                worksImport = import,
                client = client,
                catalogue = catalogue
            });

            if (result.IsFailure) return;

            var standalone = Mapper.Map<Core.Entities.StandAlone>(result.Value);
            OscarContext.StandAlones.Add(standalone);
            await OscarContext.SaveChangesAsync();
        }

        private async Task ProcessSeries(Core.Entities.WorksImport import, WorksImportRequest worksImportRequest, Client client, Core.Entities.Catalogue catalogue)
        {
            var result = await _mediator.Send(new SetWorksEntityCommand
            {
                worksImportRequest = worksImportRequest,
                worksImport = import,
                client = client,
                catalogue = catalogue
            });

            var series = Mapper.Map<Core.Entities.Series>(result.Value);

            series.Seasons = new List<Core.Entities.Season>();
            var seasonWorksList = worksImportRequest.WorksImports.Where(w => w.WorksType != null && w.WorksType.ToLower() == "season" && w.SASeriesNumber == import.SASeriesNumber);

            foreach (var seasonWorks in seasonWorksList)
            {
                result = await _mediator.Send(new SetWorksEntityCommand
                {
                    worksImportRequest = worksImportRequest,
                    worksImport = seasonWorks,
                    client = client,
                    catalogue = catalogue,
                });

                var season = Mapper.Map<Core.Entities.Season>(result.Value);

                season.Episodes = new List<Core.Entities.Episode>();
                var episodeWorksList = worksImportRequest.WorksImports.Where(w => w.WorksType != null && w.WorksType.ToLower() == "episode" && w.SASeriesNumber == seasonWorks.SASeriesNumber && w.SeasonNumber == seasonWorks.SeasonNumber);

                foreach (var episodeWork in episodeWorksList)
                {
                    result = await _mediator.Send(new SetWorksEntityCommand
                    {
                        worksImportRequest = worksImportRequest,
                        worksImport = episodeWork,
                        client = client,
                        catalogue = catalogue,
                        titleType = TitleType.Episode
                    });

                    var episode = Mapper.Map<Core.Entities.Episode>(result.Value);
                    season.Episodes.Add(episode);
                }
                series.Seasons.Add(season);
            }
            OscarContext.Series.Add(series);
            await OscarContext.SaveChangesAsync();
        }

        public async Task CheckForDuplicates(ICollection<Core.Entities.WorksImport> worksImportList, CancellationToken cancellationToken)
        {
            foreach (var chunk in worksImportList.Chunk(Constants.Default.ThreadSize))
                await Task.WhenAll(chunk.Select(work => Task.Run(() => ProcessCheck(work), cancellationToken)).ToArray()).ConfigureAwait(false);
        }

        private async Task ProcessCheck(Core.Entities.WorksImport work)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var threadSafeMediator = scopedServices.GetRequiredService<IMediator>();

            var result = await threadSafeMediator.Send(new SearchWorksQuery
            {
                Title = work.Title,
                ProductionYear = !string.IsNullOrEmpty(work.ProductionYear) ? int.Parse(work.ProductionYear) : null,
                DirectorFirstName = work.DirectorFirstName,
                DirectorLastName = work.DirectorLastName,
                StatusDiscriminator = WorksStatus.Any
            });

            work.PossibleDuplicate = result.Value.TotalRecords > 0;
        }
    }
}
