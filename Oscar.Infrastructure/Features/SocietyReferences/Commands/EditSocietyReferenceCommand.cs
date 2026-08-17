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

public class EditSocietyReferenceCommand : IRequest<Result<SocietyReferenceDto>>
{
    public int Id { get; set; }
    public int SocietyId { get; set; }
    public string? Reference { get; set; }
}

public class EditSocietyReferenceCommandHandler : AbstractBaseHandler<EditSocietyReferenceCommand, SocietyReferenceDto>
{
    public EditSocietyReferenceCommandHandler(OscarContext oscarContext, IMapper mapper, IValidator<EditSocietyReferenceCommand> validator, ILogger<EditSocietyReferenceCommand> logger) : base(oscarContext, mapper, validator, logger)
    {
    }

    protected override async Task<Result<SocietyReferenceDto>> HandleRequest(EditSocietyReferenceCommand request, CancellationToken cancellationToken)
    {
        var SocietyReference = OscarContext.SocietyReferences
            .FirstOrDefault(r => r.Id == request.Id);

        SocietyReference.Society = OscarContext.Societies.FirstOrDefault(s => s.Id == request.SocietyId);
        SocietyReference.Reference = request.Reference;
        OscarContext.Update(SocietyReference);

        await OscarContext.SaveChangesAsync(cancellationToken);

        Logger.LogInformation((int)SocietyReferenceFeatureEvent.Update, CommandResult.SUCCESS);
        return Result.Ok(Mapper.Map<SocietyReferenceDto>(SocietyReference));
    }
}
