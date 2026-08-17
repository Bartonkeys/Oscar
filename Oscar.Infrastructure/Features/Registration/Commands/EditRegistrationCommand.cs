using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;

namespace Oscar.Infrastructure.Features.Registration.Commands;

public class EditRegistrationCommand : IRequest<Result<RegistrationDisplayDto>>
{
    public int Id { get; set; }
    public int SocietyId { get; set; }
    public DateTime? DateRegistered { get; set; }
}

public class EditRegistrationCommandHandler : AbstractBaseHandler<EditRegistrationCommand, RegistrationDisplayDto>
{
    public EditRegistrationCommandHandler(OscarContext oscarContext, IMapper mapper, IValidator<EditRegistrationCommand> validator, ILogger<EditRegistrationCommand> logger) : base(oscarContext, mapper, validator, logger)
    {
    }

    protected override async Task<Result<RegistrationDisplayDto>> HandleRequest(EditRegistrationCommand request, CancellationToken cancellationToken)
    {
        var registration = OscarContext.Registrations
            .FirstOrDefault(r => r.Id == request.Id);

        registration.Society = OscarContext.Societies.FirstOrDefault(s => s.Id == request.SocietyId);
        registration.DateRegistered = request.DateRegistered;
        OscarContext.Update(registration);

        await OscarContext.SaveChangesAsync(cancellationToken);

        Logger.LogInformation((int)RegistrationFeatureEvent.Update, CommandResult.SUCCESS);
        return Result.Ok(Mapper.Map<RegistrationDisplayDto>(registration));
    }
}
