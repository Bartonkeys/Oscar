using System.Collections.Concurrent;
using System.Threading;
using AutoMapper;
using Azure.Core;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;
using Oscar.Infrastructure.Features.Common.Contracts;

namespace Oscar.Infrastructure.Features.Screenrights.Commands
{
    public class ProcessScreenrightsCommand : IRequest<Result<string>>
    {
        public Guid RequestId { get; set; }
    }

    public class ScreenrightsCommandHandler : AbstractBaseHandler<ProcessScreenrightsCommand,string>
    {
        private IExporter _exporter;
        private IImporter _importer;
        private readonly ConcurrentBag<ScreenrightsRequestDto> _exportList = new();
        private readonly ConcurrentBag<string> _registrationErrors = new();
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public ScreenrightsCommandHandler(
            OscarContext oscarContext,
            IMapper mapper,
            IValidator<ProcessScreenrightsCommand> validator,
            IExporter exporter,
            IImporter importer,
            IServiceScopeFactory serviceScopeFactory,
            ILogger<ProcessScreenrightsCommand> logger, 
            IMediator mediator)
            : base(oscarContext, mapper, validator, logger)
        {
            _exporter = exporter;
            _importer = importer;
            _serviceScopeFactory = serviceScopeFactory;
            _mapper = mapper;
            _mediator = mediator;
        }

        protected override async Task<Result<string>> HandleRequest(ProcessScreenrightsCommand request, CancellationToken cancellationToken)
        {
            var screenrightsRequest = await OscarContext.ScreenrightsRequests.FirstOrDefaultAsync(m => m.RequestID == request.RequestId, cancellationToken: cancellationToken);
            if (screenrightsRequest == null)
            {
                Logger.LogInformation((int)ScreenrightsRequestFeatureEvent.DocumentNotFound, $"Not found {request.RequestId}");
                return Result.Fail<string>(CommandResult.NOTFOUND);
            }

            screenrightsRequest.ScreenrightsRequestStatus = ScreenrightsRequestStatus.Processing;
            await OscarContext.SaveChangesAsync(cancellationToken);

            var importResult = _importer.ImportScreenrightsCsvAsList($"{screenrightsRequest.FileName}");
            if (importResult.IsFailure)
            {
                Logger.LogError((int)ScreenrightsRequestFeatureEvent.DocumentNotFound, $"Document '{screenrightsRequest.FileName}' not found in storage: {importResult.Error}");
                screenrightsRequest.ScreenrightsRequestStatus = ScreenrightsRequestStatus.Failed;
                await OscarContext.SaveChangesAsync(cancellationToken);

                return Result.Fail<string>(CommandResult.NOTFOUND);
            }

            try
            {
                foreach (var screenrights in importResult.Value)
                {
                    Console.WriteLine($"ScreenrightsReference: {screenrights.ClaimID} - Compact Ref: {screenrights.Your_Reference}");

                    if (screenrights.Your_Reference != null && screenrights.Your_Reference != String.Empty)
                    {
                        var compactRef = screenrights.Your_Reference.Remove(0, 3);

                        var matchingWork = OscarContext.Works
                            .Include(w => w.SocietyReferences)
                            .First(w => w.CompactRef != null && w.CompactRef.Equals(compactRef));
                        if (screenrights.ClaimID != null && screenrights.ClaimID != String.Empty)
                        {
                            if (matchingWork.SocietyReferences != null && matchingWork.SocietyReferences.Count > 0)
                            {
                                foreach (var societyReference in matchingWork.SocietyReferences)
                                {
                                    societyReference.Reference = screenrights.ClaimID;
                                }
                            }
                            else
                            {
                                var societyReference = new SocietyReference()
                                {
                                    Reference = screenrights.ClaimID,
                                    CompactReference = compactRef,
                                    Works = matchingWork
                                };
                                matchingWork.SocietyReferences = new List<SocietyReference>()
                                {
                                    societyReference
                                };

                            }
                        }

                        screenrightsRequest.ScreenrightsRequestStatus = ScreenrightsRequestStatus.Processed;
                        await OscarContext.SaveChangesAsync(cancellationToken);
                    }
                }

                return Result.Ok(screenrightsRequest.ToString());
            }
            catch (Exception ex)
            {
                Logger.LogError((int)FunctionEvent.ScreenrightsError, $"Processing error with document '{screenrightsRequest.FileName}': {ex.ToString}");
                return Result.Fail<string>(CommandResult.ERROR);
            }

        }
    }
}
