using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;

namespace Oscar.Infrastructure.Features.WorksImport.Commands
{
    public class DeleteWorksImportCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
    }

    public class DeleteWorksImportCommandHandler : AbstractBaseHandler<DeleteWorksImportCommand, int>
    {

        public DeleteWorksImportCommandHandler(
            OscarContext oscarContext, 
            IMapper mapper, 
            IValidator<DeleteWorksImportCommand> validator, 
            ILogger<DeleteWorksImportCommand> logger
            ) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<int>> HandleRequest(DeleteWorksImportCommand request, CancellationToken cancellationToken)
        {
            var worksImport = await OscarContext.WorksImports.FindAsync(request.Id);
            if(worksImport == null)
            {
                Logger.LogInformation((int)WorksImportRequestFeatureEvent.DeleteNotFound, $"Not found {request.Id}");
                return Result.Fail<int>(CommandResult.NOTFOUND);
            }

            var worksRightsImports = OscarContext.WorksRightsImports.Where(w => w.WorksImport != null && w.WorksImport.Id == request.Id).ToList();
            OscarContext.WorksRightsImports.RemoveRange(worksRightsImports);
            OscarContext.WorksImports.Remove(worksImport);
            var deleteResult = await OscarContext.SaveChangesAsync();

            return Result.Ok<int>(deleteResult);
        }
    }
}
