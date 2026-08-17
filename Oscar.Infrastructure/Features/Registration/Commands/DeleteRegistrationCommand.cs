using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;


namespace Oscar.Infrastructure.Features.Registration.Commands
{
    public class DeleteRegistrationCommand: IRequest<Result<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteRegistrationCommandHandler : AbstractBaseHandler<DeleteRegistrationCommand, string>
    {
        public DeleteRegistrationCommandHandler(OscarContext oscarContext, IMapper mapper, IValidator<DeleteRegistrationCommand> validator, ILogger<DeleteRegistrationCommand> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<string>> HandleRequest(DeleteRegistrationCommand request, CancellationToken cancellationToken)
        {
            var registration = OscarContext.Registrations
                .Include(e => e.RegistrationBatch)
                .FirstOrDefault(s => s.Id == request.Id);

            if (registration == null)
            {
                Logger.LogInformation((int)RegistrationFeatureEvent.DeleteNotFound, $"Not found {request.Id}");
                return Result.Fail<string>(CommandResult.NOTFOUND);
            }

            OscarContext.Registrations.Remove(registration);
            await OscarContext.SaveChangesAsync(cancellationToken);

            Logger.LogInformation((int)RegistrationFeatureEvent.Delete, CommandResult.SUCCESS);
            return Result.Ok(CommandResult.SUCCESS);
        }

        
    }
}
