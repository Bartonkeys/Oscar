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

namespace Oscar.Infrastructure.Features.Screenrights.Commands
{
    public class AddScreenrightsRequestCommand : IRequest<Result>
    {
        public ScreenrightsRequestDto ScreenrightsRequestDto { get; set; }
    }

    public class AddScreenrightsRequestCommandHandler : SimpleAbstractBaseHandler<AddScreenrightsRequestCommand>
    {
        private IQueueService _queueService;
        private IContainerService _containerService;

        public AddScreenrightsRequestCommandHandler(
            OscarContext oscarContext,
            IMapper mapper,
            IValidator<AddScreenrightsRequestCommand> validator,
            ILogger<AddScreenrightsRequestCommand> logger,
            IContainerService containerService,
            IQueueService queueService
            ) : base(oscarContext, mapper, validator, logger)
        {
            _containerService = containerService;
            _queueService = queueService;
        }

        protected override async Task<Result> HandleRequest(AddScreenrightsRequestCommand request, CancellationToken cancellationToken)
        {
            var screenrightsRequest = Mapper.Map<ScreenrightsRequest>(request.ScreenrightsRequestDto);

            screenrightsRequest.ScreenrightsRequestStatus = ScreenrightsRequestStatus.Scheduled;
            screenrightsRequest.RequestID = Guid.NewGuid();

            OscarContext.Add(screenrightsRequest);

            await OscarContext.SaveChangesAsync(cancellationToken);

            var uploadResult = await _containerService.UploadAsync(request.ScreenrightsRequestDto.FormFile, ContainerName.SCREENRIGHTS, screenrightsRequest.Id, null, null, cancellationToken);
            if (uploadResult.IsSuccess)
            {

                var queueResult = await _queueService.SendAsync(QueueName.SCREENRIGHTS, screenrightsRequest.RequestID.ToString(), cancellationToken);
                if (queueResult.IsSuccess)
                {
                    screenrightsRequest.FileName = uploadResult.Value;
                    await OscarContext.SaveChangesAsync(cancellationToken);
                    Logger.LogInformation((int)ScreenrightsRequestFeatureEvent.Add, CommandResult.SUCCESS);
                    return Result.Ok(Mapper.Map<ScreenrightsRequestDto>(screenrightsRequest));
                }
            }
            screenrightsRequest.ScreenrightsRequestStatus = ScreenrightsRequestStatus.Failed;
            await OscarContext.SaveChangesAsync(cancellationToken);
            return Result.Fail<ScreenrightsRequestDto>(CommandResult.ERROR);
        }

        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
       
    }
}
