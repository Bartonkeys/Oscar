using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;

namespace Oscar.Infrastructure.Features.Registration.Commands;

public class AddRegistrationCommand : IRequest<Result<RegistrationDisplayDto>>
{
    public int SocietyId { get; set; }
    public DateTime? DateRegistered { get; set; }
    public int? ClientId { get; set; }
    public int WorksId { get; set; }
}

public class AddRegistrationCommandHandler : AbstractBaseHandler<AddRegistrationCommand, RegistrationDisplayDto>
{
    private readonly IMediator _mediator;

    public AddRegistrationCommandHandler(OscarContext oscarContext, IMapper mapper, IValidator<AddRegistrationCommand> validator, ILogger<AddRegistrationCommand> logger, IMediator mediator) : base(oscarContext, mapper, validator, logger)
    {
        _mediator = mediator;
    }

    protected override async Task<Result<RegistrationDisplayDto>> HandleRequest(AddRegistrationCommand request, CancellationToken cancellationToken)
    {
        var newRegistrationBatch = new Core.Entities.RegistrationBatch();

        var registrationBatch = OscarContext.RegistrationBatches.FirstOrDefault(x => x.BatchId == new Guid(Core.Common.Constants.ManualEntryRegistrationBatchId));

        if (registrationBatch == null)
        {
            newRegistrationBatch = new Core.Entities.RegistrationBatch
            {
                BatchId = new Guid(Core.Common.Constants.ManualEntryRegistrationBatchId),
                DateRegistered = DateTime.UtcNow,
                Notes = "This is used to keep all manual registrations under same batch",
                RegisterStatus = RegisterStatus.Registered,
                DoNotRegister= false
            };

            OscarContext.Add(newRegistrationBatch);
        }

        var registration = new Core.Entities.Registration
        {
            RegistrationBatch = registrationBatch ?? newRegistrationBatch,
            Works = OscarContext.Works.FirstOrDefault(x => x.Id == request.WorksId),
            Client = OscarContext.Clients.FirstOrDefault(x => x.Id == request.ClientId),
            Society = OscarContext.Societies.FirstOrDefault(x => x.Id == request.SocietyId),
            DateRegistered = request.DateRegistered,
            RegisterStatus = RegisterStatus.Registered,
        };

        OscarContext.Add(registration);
        await OscarContext.SaveChangesAsync(cancellationToken);

        Logger.LogInformation((int)RegistrationFeatureEvent.Add, CommandResult.SUCCESS);
        return Result.Ok(Mapper.Map<RegistrationDisplayDto>(registration));
    }
}
