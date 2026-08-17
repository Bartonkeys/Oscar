using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;
using Oscar.Infrastructure.Features.Common.Contracts;
using Oscar.Infrastructure.Features.WorksImport.Services;

namespace Oscar.Infrastructure.Features.WorksImport.Commands
{
    public class AddWorksImportRequestCommand : IRequest<Result>
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

    public class AddWorksImportRequestCommandHandler : SimpleAbstractBaseHandler<AddWorksImportRequestCommand>
    {
        private readonly IQueueService _queueService;
        private IWorksImportService _worksImportService;

        public AddWorksImportRequestCommandHandler(
            OscarContext oscarContext, 
            IMapper mapper, 
            IValidator<AddWorksImportRequestCommand> validator, 
            ILogger<AddWorksImportRequestCommand> logger,
            IQueueService queueService, IWorksImportService worksImportService) : base(oscarContext, mapper, validator, logger)
        {
            _queueService = queueService;
            _worksImportService = worksImportService;
        }

        protected override async Task<Result> HandleRequest(AddWorksImportRequestCommand request, CancellationToken cancellationToken)
        {
            var worksImportRequest = Mapper.Map<WorksImportRequest>(request.WorksImportRequestAddDto);
            worksImportRequest.Reference = String.Empty;
            worksImportRequest.Status = WorksImportRequestStatus.Pending;

            var client = await OscarContext
                .Clients
                .Include(c => c.Catalogues)
                .Include(r => r.Rights)
                .FirstOrDefaultAsync(c => c.Id == request.WorksImportRequestAddDto.ClientId, cancellationToken: cancellationToken);

            if (client == null)
            {
                Logger.LogError((int)WorksImportRequestFeatureEvent.DocumentNotFound, $"Client {request.WorksImportRequestAddDto.ClientId} not found for works import request {worksImportRequest.Id}");
                worksImportRequest.Status = WorksImportRequestStatus.Error;
                await OscarContext.SaveChangesAsync(cancellationToken);
                return Result.Fail("Client not found");
            }

            Oscar.Core.Entities.Catalogue? catalogue = null;
            if (client.Catalogues.Any())
            {
                catalogue = client.Catalogues.Count == 1
                    ? client.Catalogues.First()
                    : client.Catalogues.FirstOrDefault(c => c.Id == request.WorksImportRequestAddDto.CatalogueId);
                worksImportRequest.Catalogue = catalogue;
            }

            worksImportRequest.Client = client;

            var records = _worksImportService.WorksImportDtoListFromFile(request.WorksImportRequestAddDto.FormFile, request.WorksImportRequestAddDto.IsAgicoa);

            var lockObject = new object();
            worksImportRequest.WorksImports = new List<Core.Entities.WorksImport>();
            foreach (var chunk in records.Chunk(10))
            {
                var importTasks = chunk.Select(worksImportDto => Task.Run(() =>
                {
                    var worksImport = Mapper.Map<Core.Entities.WorksImport>(worksImportDto);
                    lock (lockObject)
                    {
                        worksImportRequest.WorksImports.Add(worksImport);
                    }
                }, cancellationToken));

                await Task.WhenAll(importTasks);
            }

            OscarContext.Add(worksImportRequest);
            await OscarContext.SaveChangesAsync(cancellationToken);

            var worksImportQueueDto = new WorksImportQueueDto
            {
                Id = worksImportRequest.Id,
                Status = worksImportRequest.Status
            };

            var queueResult = await _queueService.SendAsync(QueueName.WORKSIMPORT, JsonConvert.SerializeObject(worksImportQueueDto), cancellationToken);
            return queueResult.IsFailure ? Result.Fail(queueResult.Error) : Result.Ok();
        }
    }
}
