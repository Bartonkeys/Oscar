using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;
using Oscar.Infrastructure.Features.WorksImport.Services;

namespace Oscar.Infrastructure.Features.WorksImport.Commands
{
    public class AddEpisodeImportRequestCommand : IRequest<Result<int>>
    {
        private string _error = "";
        public WorksImportRequestAddDto WorksImportRequestAddDto { get; set; }

        public string GetError()
        {
            return _error;
        }
        public void SetError(string error)
        {
            _error = error;
        }
    }

    public class AddEpisodeImportRequestCommandHandler : AbstractBaseHandler<AddEpisodeImportRequestCommand, int>
    {
        private readonly IWorksImportService WorksImportService;


        public AddEpisodeImportRequestCommandHandler(
            IWorksImportService worksImportService,
            OscarContext oscarContext, 
            IMapper mapper, 
            IValidator<AddEpisodeImportRequestCommand> validator, 
            ILogger<AddEpisodeImportRequestCommand> logger) : base(oscarContext, mapper, validator, logger) 
        {
            WorksImportService = worksImportService;
        }

        protected override async Task<Result<int>> HandleRequest(AddEpisodeImportRequestCommand request, CancellationToken cancellationToken)
        {

            var client = await OscarContext.Clients.Include(c => c.Catalogues).FirstOrDefaultAsync(c => c.Id == request.WorksImportRequestAddDto.ClientId);
            if (client == null)
            {
                Logger.LogError((int)WorksImportRequestFeatureEvent.DocumentNotFound, $"Client {request.WorksImportRequestAddDto.ClientId} not found for episode import");
                return Result.Fail<int>("Client not found");
            }

            Oscar.Core.Entities.Catalogue? catalogue = null;
            if (client.Catalogues.Any())
            {
                catalogue = client.Catalogues.Count == 1 ? client.Catalogues.First() : client.Catalogues.FirstOrDefault(c => c.Id == request.WorksImportRequestAddDto.CatalogueId);
            }

            var records = WorksImportService.EpisodeImportDtoListFromFile(request.WorksImportRequestAddDto.FormFile);

            foreach (var episodeImport in records)
            {
                var episode = new Core.Entities.Episode
                {
                    Number = int.Parse(episodeImport.EpisodeNumber),
                    Titles = new List<WorksTitle>() { new WorksTitle() { Title = episodeImport.EpisodeTitle } },
                    SeasonId = int.Parse(episodeImport.OscarSeasonRef),
                    Clients = new List<Client>()
                };
                episode.Clients.Add(client);
                if(catalogue != null)
                {
                    episode.Catalogues = new List<Core.Entities.Catalogue>
                    {
                        catalogue
                    };
                }
                OscarContext.Episodes.Add(episode);
            }
            await OscarContext.SaveChangesAsync();
            return Result.Ok<int>(records.Count);
        }
        

       

    }
}
