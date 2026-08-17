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
using Oscar.Infrastructure.Features.Common.Services;

namespace Oscar.Infrastructure.Features.Equivalence.Commands
{
    public class AddEquivalenceRequestCommand : IRequest<Result>
    {
        public EquivalenceRequestDto EquivalenceRequestDto { get; set; }
    }

    public class AddEquivalenceRequestCommandHandler : SimpleAbstractBaseHandler<AddEquivalenceRequestCommand>
    {
        private IQueueService _queueService;
        private IContainerService _containerService;

        public AddEquivalenceRequestCommandHandler(
            OscarContext oscarContext,
            IMapper mapper,
            IValidator<AddEquivalenceRequestCommand> validator,
            ILogger<AddEquivalenceRequestCommand> logger,
            IContainerService containerService,
            IQueueService queueService
            ) : base(oscarContext, mapper, validator, logger)
        {
            _containerService = containerService;
            _queueService = queueService;
        }

        protected override async Task<Result> HandleRequest(AddEquivalenceRequestCommand request, CancellationToken cancellationToken)
        {
            var equivalenceRequest = Mapper.Map<EquivalenceRequest>(request.EquivalenceRequestDto);

            equivalenceRequest.EquivalenceRequestStatus = EquivalenceRequestStatus.Scheduled;
            equivalenceRequest.RequestID = Guid.NewGuid();

            OscarContext.Add(equivalenceRequest);

            await OscarContext.SaveChangesAsync(cancellationToken);

            var uploadResult = await _containerService.UploadAsync(request.EquivalenceRequestDto.FormFile, ContainerName.EQUIVALENCE, equivalenceRequest.Id, null, null, cancellationToken);
            if (uploadResult.IsSuccess)
            {

                var queueResult = await _queueService.SendAsync(QueueName.EQUIVALENCE, equivalenceRequest.RequestID.ToString(), cancellationToken);
                if (queueResult.IsSuccess)
                {
                    equivalenceRequest.FileName = uploadResult.Value;
                    await OscarContext.SaveChangesAsync(cancellationToken);
                    Logger.LogInformation((int)EquivalenceRequestFeatureEvent.Add, CommandResult.SUCCESS);
                    return Result.Ok(Mapper.Map<EquivalenceRequestDto>(equivalenceRequest));
                }
            }
            equivalenceRequest.EquivalenceRequestStatus = EquivalenceRequestStatus.Failed;
            await OscarContext.SaveChangesAsync(cancellationToken);
            return Result.Fail<EquivalenceRequestDto>(CommandResult.ERROR);
        }

        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
       
    }
}
