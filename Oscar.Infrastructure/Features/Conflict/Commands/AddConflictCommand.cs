using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;

namespace Oscar.Infrastructure.Features.Conflict.Commands;

public class AddConflictCommand : IRequest<Result<ConflictDto>>
{
    public int SocietyId { get; set; }
    public string Notes { get; set; }
    public DateTime? NotationDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int WorksId { get; set; }
    public bool Internal { get; set; }
}

public class AddConflictCommandHandler : AbstractBaseHandler<AddConflictCommand, ConflictDto>
{
    private readonly IMediator _mediator;

    public AddConflictCommandHandler(OscarContext oscarContext, IMapper mapper, IValidator<AddConflictCommand> validator, ILogger<AddConflictCommand> logger, IMediator mediator) : base(oscarContext, mapper, validator, logger)
    {
        _mediator = mediator;
    }

    protected override async Task<Result<ConflictDto>> HandleRequest(AddConflictCommand request, CancellationToken cancellationToken)
    {
        var Conflict = new Core.Entities.Conflict
        {
            Notes = request.Notes,
            NotationDate= request.NotationDate,
            Works = OscarContext.Works.FirstOrDefault(x => x.Id == request.WorksId),
            Society = OscarContext.Societies.FirstOrDefault(x => x.Id == request.SocietyId),
            Internal= request.Internal
        };

        OscarContext.Add(Conflict);
        await OscarContext.SaveChangesAsync(cancellationToken);

        Logger.LogInformation((int)ConflictFeatureEvent.Add, CommandResult.SUCCESS);
        return Result.Ok(Mapper.Map<ConflictDto>(Conflict));
    }
}
