using AutoMapper;
using BartonKeys.Functional;
using EFCore.BulkExtensions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;
using Oscar.Infrastructure.Features.Common.Contracts;

namespace Oscar.Infrastructure.Features.Registration.Commands
{
    public class AddRegistrationBatchCommand : IRequest<Result<RegistrationBatchCreateDto>>
    {
        public int? ClientId { get; set; }
        public int? CatalogueId { get; set; }
        public int? SocietyId { get; set; }
        public bool IncludePreviouslyRegisteredWorks { get; set; }
        public RegistrationBatchCreateDto RegistrationBatchCreateDto { get; set; }
        public bool IsAllClients { get; set; }
        public bool DoNotRegister { get; set; }
        public IEnumerable<int>? UserSelectedWorkIds { get; set; }
    }

    public class AddRegistrationBatchCommandCommandHandler : AbstractBaseHandler<AddRegistrationBatchCommand, RegistrationBatchCreateDto>
    {
        private IQueueService _queueService;

        public AddRegistrationBatchCommandCommandHandler(
            OscarContext oscarContext,
            IMapper mapper,
            IValidator<AddRegistrationBatchCommand> validator,
            ILogger<AddRegistrationBatchCommand> logger,
            IQueueService queueService
            ) : base(oscarContext, mapper, validator, logger)
        {
            _queueService = queueService;
        }

        protected override async Task<Result<RegistrationBatchCreateDto>> HandleRequest(AddRegistrationBatchCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var registrationDate = DateTime.Now;
                var registrationBatch = new RegistrationBatch
                {
                    RegisterStatus = RegisterStatus.Batch_Created,
                    DateRegistered = registrationDate,
                    BatchId = Guid.NewGuid(),
                    SocietyId = request.SocietyId,
                    ClientId = request.ClientId,
                    CatalogueId = request.CatalogueId,
                    IncludePreviouslyRegistered = request.IncludePreviouslyRegisteredWorks,
                    IsAllClients = request.IsAllClients,
                    DoNotRegister = request.DoNotRegister
                };
                OscarContext.Add(registrationBatch);
                await OscarContext.SaveChangesAsync(cancellationToken);

                if (request.UserSelectedWorkIds != null && request.UserSelectedWorkIds.Any())
                {
                    var client = await OscarContext.Clients.FindAsync(request.ClientId);
                    var society = await OscarContext.Societies.FindAsync(request.SocietyId);
                    
                    var userDefinedRegistrations = request.UserSelectedWorkIds.Select(workId => new Core.Entities.Registration
                    {
                        RegistrationBatch = registrationBatch,
                        RegisterStatus = RegisterStatus.UserSelected,
                        Client = client,
                        Works = OscarContext.Works.Find(workId),
                        Society = society,
                        RegisterType = RegisterType.Zero,
                        ModifiedBy = "RegistrationFunction",
                        DateRegistered = registrationDate
                    });

                    await OscarContext.BulkInsertAsync(userDefinedRegistrations.ToList(), cancellationToken: cancellationToken);
                }

                var queueResult = await _queueService.SendAsync(QueueName.REGISTRATION, registrationBatch.BatchId.ToString(), cancellationToken);
                if (queueResult.IsSuccess)
                {
                    registrationBatch.RegisterStatus = RegisterStatus.Scheduled;
                    await OscarContext.SaveChangesAsync(cancellationToken);
                    Logger.LogInformation((int)RegistrationFeatureEvent.AddedToQueue, CommandResult.SUCCESS);
                    var registrationBatchDto = Mapper.Map<RegistrationBatchCreateDto>(registrationBatch);
                    return Result.Ok(registrationBatchDto);
                }
                registrationBatch.RegisterStatus = RegisterStatus.Error;
                Logger.LogInformation((int)RegistrationFeatureEvent.Error, CommandResult.ERROR);
                await OscarContext.SaveChangesAsync(cancellationToken);
                return Result.Fail<RegistrationBatchCreateDto>(CommandResult.ERROR);
            }
            catch (Exception ex)
            {
                Logger.LogInformation((int)RegistrationFeatureEvent.Error, CommandResult.ERROR, ex.Message);
                return Result.Fail<RegistrationBatchCreateDto>(CommandResult.ERROR);
            }
        }
    }
}
