using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;

namespace Oscar.Infrastructure.Features.SocietyReferences.Commands;

public class AddSocietyReferenceCommand : IRequest<Result<SocietyReferenceDto>>
{
    public int SocietyId { get; set; }
    public int WorksId { get; set; }
    public string? Reference { get; set; }
}

public class AddSocietyReferenceCommandHandler : AbstractBaseHandler<AddSocietyReferenceCommand, SocietyReferenceDto>
{
    private readonly IMediator _mediator;

    public AddSocietyReferenceCommandHandler(OscarContext oscarContext, IMapper mapper, IValidator<AddSocietyReferenceCommand> validator, ILogger<AddSocietyReferenceCommand> logger, IMediator mediator) : base(oscarContext, mapper, validator, logger)
    {
        _mediator = mediator;
    }

    protected override async Task<Result<SocietyReferenceDto>> HandleRequest(AddSocietyReferenceCommand request, CancellationToken cancellationToken)
    {
        var SocietyReference = new Core.Entities.SocietyReference
        {
            Works = OscarContext.Works.FirstOrDefault(x => x.Id == request.WorksId),
            Society = OscarContext.Societies.FirstOrDefault(x => x.Id == request.SocietyId),
            Reference = request.Reference
        };

        OscarContext.Add(SocietyReference);
        await OscarContext.SaveChangesAsync(cancellationToken);

        Logger.LogInformation((int)SocietyReferenceFeatureEvent.Add, CommandResult.SUCCESS);
        return Result.Ok(Mapper.Map<SocietyReferenceDto>(SocietyReference));
    }
}
