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

namespace Oscar.Infrastructure.Features.Conflict.Commands;

public class EditConflictCommand : IRequest<Result<ConflictDto>>
{
    public int Id { get; set; }
    public int SocietyId { get; set; }
    public string Notes { get; set; }
    public DateTime? NotationDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool Internal { get; set; }
}

public class EditConflictCommandHandler : AbstractBaseHandler<EditConflictCommand, ConflictDto>
{
    public EditConflictCommandHandler(OscarContext oscarContext, IMapper mapper, IValidator<EditConflictCommand> validator, ILogger<EditConflictCommand> logger) : base(oscarContext, mapper, validator, logger)
    {
    }

    protected override async Task<Result<ConflictDto>> HandleRequest(EditConflictCommand request, CancellationToken cancellationToken)
    {
        var conflict = OscarContext.Conflicts
            .FirstOrDefault(r => r.Id == request.Id);

        conflict.Society = OscarContext.Societies.FirstOrDefault(s => s.Id == request.SocietyId);
        conflict.Notes = request.Notes;
        conflict.NotationDate = request.NotationDate;
        conflict.Internal = request.Internal;
        OscarContext.Update(conflict);

        await OscarContext.SaveChangesAsync(cancellationToken);

        Logger.LogInformation((int)ConflictFeatureEvent.Update, CommandResult.SUCCESS);
        return Result.Ok(Mapper.Map<ConflictDto>(conflict));
    }
}
