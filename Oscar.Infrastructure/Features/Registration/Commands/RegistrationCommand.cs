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
using Oscar.Infrastructure.Features.Registration.Contracts;

namespace Oscar.Infrastructure.Features.Registration.Commands
{
    public class RegistrationCommand : IRequest<Result<string>>
    {
        public Guid BatchId { get; set; }
        public int ClientId { get; set; }
    }

    public class RegistrationCommandHandler : AbstractBaseHandler<RegistrationCommand,string>
    {
        private IExporter _exporter;
        private readonly ConcurrentBag<RegistrationCreateDto> _exportList = new();
        private readonly ConcurrentBag<string> _registrationErrors = new();
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public RegistrationCommandHandler(
            OscarContext oscarContext,
            IMapper mapper,
            IValidator<RegistrationCommand> validator,
            IExporter exporter,
            IServiceScopeFactory serviceScopeFactory,
            ILogger<RegistrationCommand> logger, 
            IMediator mediator)
            : base(oscarContext, mapper, validator, logger)
        {
            _exporter = exporter;
            _serviceScopeFactory = serviceScopeFactory;
            _mapper = mapper;
            _mediator = mediator;
        }

        protected override async Task<Result<string>> HandleRequest(RegistrationCommand request, CancellationToken cancellationToken)
        {
            var registrationBatch = await OscarContext.RegistrationBatches.FirstOrDefaultAsync(m => m.BatchId == request.BatchId, cancellationToken: cancellationToken);
            if (registrationBatch == null)
            {
                Logger.LogInformation((int)RegistrationFeatureEvent.BatchNotFound, $"Not found {request.BatchId}");
                return Result.Fail<string>(CommandResult.NOTFOUND);
            }

            registrationBatch.RegisterStatus = RegisterStatus.Processing;
            await OscarContext.SaveChangesAsync(cancellationToken);

            var society = await OscarContext.Societies.SingleAsync(s => s.Id == registrationBatch.SocietyId, cancellationToken: cancellationToken);

            Result<string> registrationResult;
            switch (society.Name)
            {
                case "AGICOA":
                    registrationResult = await _mediator.Send(new AgicoaRegistrationCommand{BatchId = request.BatchId, ClientId = request.ClientId}, cancellationToken);
                    if (registrationResult.IsFailure)
                        return Result.Fail<string>(registrationResult.Error);
                    break;
                case "SUISSIMAGE":
                    registrationResult = await _mediator.Send(new SuisseImageCommand() { BatchId = request.BatchId, ClientId = request.ClientId}, cancellationToken);
                    if (registrationResult.IsFailure)
                        return Result.Fail<string>(registrationResult.Error);
                    break;
                case "SCREENRIGHTS":
                    registrationResult = await _mediator.Send(new ScreenrightsRegistrationCommand() { BatchId = request.BatchId, ClientId = request.ClientId }, cancellationToken);
                    if (registrationResult.IsFailure)
                        return Result.Fail<string>(registrationResult.Error);
                    break;
                case "CCC":
                    registrationResult = await _mediator.Send(new CCCRegistrationCommand() { BatchId = request.BatchId, ClientId = request.ClientId }, cancellationToken);
                    if (registrationResult.IsFailure)
                        return Result.Fail<string>(registrationResult.Error);
                    break;
                case "CMC":
                    registrationResult = await _mediator.Send(new CMCRegistrationCommand() { BatchId = request.BatchId, ClientId = request.ClientId }, cancellationToken);
                    if (registrationResult.IsFailure)
                        return Result.Fail<string>(registrationResult.Error);
                    break;
                case "MPLC":
                    registrationResult = await _mediator.Send(new MPLCRegistrationCommand() { BatchId = request.BatchId, ClientId = request.ClientId }, cancellationToken);
                    if (registrationResult.IsFailure)
                        return Result.Fail<string>(registrationResult.Error);
                    break;
                case "CRC":
                    registrationResult = await _mediator.Send(new CRCRegistrationCommand() { BatchId = request.BatchId, ClientId = request.ClientId }, cancellationToken);
                    if (registrationResult.IsFailure)
                        return Result.Fail<string>(registrationResult.Error);
                    break;
                case "EGEDA":
                    registrationResult = await _mediator.Send(new EGEDARegistrationCommand() { BatchId = request.BatchId, ClientId = request.ClientId }, cancellationToken);
                    if (registrationResult.IsFailure)
                        return Result.Fail<string>(registrationResult.Error);
                    break;
                case "GWFF":
                    registrationResult = await _mediator.Send(new GWFFRegistrationCommand() { BatchId = request.BatchId, ClientId = request.ClientId }, cancellationToken);
                    if (registrationResult.IsFailure)
                        return Result.Fail<string>(registrationResult.Error);
                    break;
                case "MPA":
                    registrationResult = await _mediator.Send(new MPARegistrationCommand() { BatchId = request.BatchId, ClientId = request.ClientId }, cancellationToken);
                    if (registrationResult.IsFailure)
                        return Result.Fail<string>(registrationResult.Error);
                    break;
                case "UPFAR ARGOA":
                    registrationResult = await _mediator.Send(new UpfarArgoaRegistrationCommand() { BatchId = request.BatchId, ClientId = request.ClientId }, cancellationToken);
                    if (registrationResult.IsFailure)
                        return Result.Fail<string>(registrationResult.Error);
                    break;
                default:
                    return Result.Fail<string>("Not supported");
            }

            if (!registrationBatch.IsAllClients)
            {
                registrationBatch.RegisterStatus = RegisterStatus.Batch_Export_Success;
                await OscarContext.SaveChangesAsync(cancellationToken);
            }

            return Result.Ok(registrationResult.Value);

        }
    }
}
