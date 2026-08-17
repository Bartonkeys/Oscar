using System.Collections.Concurrent;
using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;
using Oscar.Infrastructure.Features.Common.Contracts;
using Oscar.Infrastructure.Features.Registration.Contracts;

namespace Oscar.Infrastructure.Features.Registration.Commands
{
    public class AgicoaRegistrationCommand : IRequest<Result<string>>
    {
        public Guid BatchId { get; set; }
        public int ClientId { get; set; }
    }

    public class AgicoaRegistrationCommandHandler: AbstractBaseHandler<AgicoaRegistrationCommand, string>
    {
        private IExporter _exporter;
        private IRegistrationService<RegistrationWorksAgicoaExport> _registrationService;
        private readonly ConcurrentBag<string> _registrationErrors = new();

        public AgicoaRegistrationCommandHandler(
            OscarContext oscarContext,
            IMapper mapper,
            IValidator<AgicoaRegistrationCommand> validator,
            IExporter exporter,
            IRegistrationService<RegistrationWorksAgicoaExport> registrationService,
            ILogger<AgicoaRegistrationCommand> logger)
            : base(oscarContext, mapper, validator, logger)
        {
            _exporter = exporter;
            _registrationService = registrationService;
        }

        protected override async Task<Result<string>> HandleRequest(AgicoaRegistrationCommand request, CancellationToken cancellationToken)
        {
            var strategy = OscarContext.Database.CreateExecutionStrategy();
            Result<RegistrationWorksAgicoaExport> registrationResult = null;

            var registrationBatch = await OscarContext.RegistrationBatches.FirstOrDefaultAsync(m => m.BatchId == request.BatchId, cancellationToken: cancellationToken);

            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = OscarContext.Database.BeginTransaction())
                {
                    registrationResult = await _registrationService.Create(registrationBatch, request.ClientId);
                    if (registrationResult.IsFailure)
                    {
                        transaction.Rollback();
                    }
                    else
                    {
                        registrationBatch.RegisterStatus = RegisterStatus.Batch_Complete;
                        await OscarContext.SaveChangesAsync(cancellationToken);
                        Logger.LogTrace((int)RegistrationFeatureEvent.BatchComplete, registrationBatch.BatchId.ToString());
                        transaction.Commit();
                    }
                }
            });

            if (registrationResult != null && registrationResult.IsFailure)
            {
                registrationBatch.RegisterStatus = RegisterStatus.Errors_Within_Batch;
                registrationBatch.Notes = string.Join(", ", registrationResult.Error);

                await OscarContext.SaveChangesAsync(cancellationToken);
                Logger.LogError((int)RegistrationFeatureEvent.ErrorsWithinBatch, string.Join(", ", _registrationErrors.Select(x => x)));
                return Result.Fail<string>(registrationResult.Error);
            }

            var exportResult = _exporter.ExportRegistrations(registrationResult.Value);

            if (exportResult.IsFailure)
            {
                registrationBatch.RegisterStatus = RegisterStatus.Batch_Export_Failed;
                await OscarContext.SaveChangesAsync(cancellationToken);
                Logger.LogError((int)RegistrationFeatureEvent.BatchExportFailed, exportResult.ToString());
                return Result.Fail<string>(exportResult.Error);
            }

            registrationBatch.FileName = registrationResult.Value.FileName;
            await OscarContext.SaveChangesAsync(cancellationToken);
            return Result.Ok(registrationResult.Value.FileName);
        }
    }
}
