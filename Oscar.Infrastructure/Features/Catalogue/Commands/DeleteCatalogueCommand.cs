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


namespace Oscar.Infrastructure.Features.Catalogue.Commands
{
    public class DeleteCatalogueCommand: IRequest<Result<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteCatalogueCommandHandler : AbstractBaseHandler<DeleteCatalogueCommand, string>
    {
        public DeleteCatalogueCommandHandler(OscarContext oscarContext, IMapper mapper, IValidator<DeleteCatalogueCommand> validator, ILogger<DeleteCatalogueCommand> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<string>> HandleRequest(DeleteCatalogueCommand request, CancellationToken cancellationToken)
        {
            var catalogue = await OscarContext.Catalogues
                .Include(c => c.Client)
                .Include(c => c.Works)
                .Include(c => c.WorksImportRequests)
                .Include(c => c.Rights)
                .Include(c => c.SocietyReferences)
                .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

            if (catalogue == null)
            {
                Logger.LogInformation((int)CatalogueFeatureEvent.DeleteNotFound, $"Not found {request.Id}");
                return Result.Fail<string>(CommandResult.NOTFOUND);
            }
            else
            {
                var clientCatalogueId = OscarContext.Catalogues.Where(c => c.Client.Id == catalogue.Client.Id).Min(c => c.Id);
                if (clientCatalogueId == request.Id)
                {
                    Logger.LogInformation((int)CatalogueFeatureEvent.Delete, CommandResult.ERROR);
                    return Result.Fail<string>("Delete failed - client catalogue cannot be deleted");
                }
            }

            var errorString = "Delete failed - there are associated records including: ";
            bool deleteError = false;

            if (catalogue.Works.Any()) { deleteError = true;  errorString += "Works "; }
            if (catalogue.WorksImportRequests.Any()) { deleteError = true; errorString += "Imports "; }
            if (catalogue.Rights!= null && catalogue.Rights.Any()) { deleteError = true; errorString += "Rights "; }
            if (catalogue.SocietyReferences.Any()) { deleteError = true; errorString += "SocietyReferences "; }

            if (deleteError)
            {
                Logger.LogInformation((int)CatalogueFeatureEvent.Delete, CommandResult.ERROR);
                return Result.Fail<string>(errorString);
            }

            OscarContext.Catalogues.Remove(catalogue);
            await OscarContext.SaveChangesAsync(cancellationToken);

            Logger.LogInformation((int)CatalogueFeatureEvent.Delete, CommandResult.SUCCESS);
            return Result.Ok(CommandResult.SUCCESS);
        }

        
    }
}
