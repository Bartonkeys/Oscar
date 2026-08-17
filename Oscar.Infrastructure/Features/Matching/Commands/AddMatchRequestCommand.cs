using AutoMapper;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;
using Oscar.Infrastructure.Features.Common.Contracts;
using Oscar.Infrastructure.Features.Matching.Contracts;

namespace Oscar.Infrastructure.Features.Matching.Commands
{
    public class AddMatchRequestCommand : IRequest<Result<MatchRequestDto>>
    {
        public MatchRequestAddDto MatchRequestAddDto { get; set; }
    }

    public class AddMatchRequestCommandHandler : AbstractBaseHandler<AddMatchRequestCommand, MatchRequestDto>
    {
        private IQueueService _queueService;
        private IContainerService _containerService;

        public AddMatchRequestCommandHandler(
            OscarContext oscarContext, 
            IMapper mapper, 
            IValidator<AddMatchRequestCommand> validator, 
            ILogger<AddMatchRequestCommand> logger,
            IContainerService containerService,
            IQueueService queueService
            ) : base(oscarContext, mapper, validator, logger)
        {
            _containerService = containerService;
            _queueService = queueService;
        }

        protected override async Task<Result<MatchRequestDto>> HandleRequest(AddMatchRequestCommand request, CancellationToken cancellationToken)
        {
            var matchRequest = Mapper.Map<Core.Entities.MatchRequest>(request.MatchRequestAddDto);
            matchRequest.Status = MatchRequestStatus.Pending;
            matchRequest.Reference = String.Empty;
            OscarContext.Add(matchRequest);

            await OscarContext.SaveChangesAsync(cancellationToken);

            var uploadResult = await _containerService.UploadAsync(request.MatchRequestAddDto.FormFile, ContainerName.MATCH, matchRequest.Id, ".csv", null, cancellationToken);
            if (uploadResult.IsSuccess)
            {
                var queueResult = await _queueService.SendAsync(QueueName.MATCH, uploadResult.Value, cancellationToken);
                if (queueResult.IsSuccess)
                {
                    matchRequest.Reference = uploadResult.Value;
                    await OscarContext.SaveChangesAsync(cancellationToken);
                    Logger.LogInformation((int)MatchRequestFeatureEvent.Add, CommandResult.SUCCESS);
                    return Result.Ok(Mapper.Map<MatchRequestDto>(matchRequest));
                }
            }
            matchRequest.Status = MatchRequestStatus.Error;
            await OscarContext.SaveChangesAsync(cancellationToken);
            return Result.Fail<MatchRequestDto>(CommandResult.ERROR);
        }

        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
    }
}
