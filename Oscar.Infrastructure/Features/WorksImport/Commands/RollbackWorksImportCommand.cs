using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;

namespace Oscar.Infrastructure.Features.WorksImport.Commands
{
    public class RollbackWorksImportCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
    }

    public class RollbackWorksImportCommandHandler : AbstractBaseHandler<RollbackWorksImportCommand, int>
    {

        public RollbackWorksImportCommandHandler(
            OscarContext oscarContext,
            IMapper mapper,
            IValidator<RollbackWorksImportCommand> validator,
            ILogger<RollbackWorksImportCommand> logger
        ) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<int>> HandleRequest(RollbackWorksImportCommand request,
            CancellationToken cancellationToken)
        {
            var worksImportRequest = await OscarContext.WorksImportRequests.FindAsync(request.Id);
            if (worksImportRequest == null)
            {
                Logger.LogInformation((int)WorksImportRequestFeatureEvent.UpdateNotFound,
                    $"Not found {request.Id}");
                return Result.Fail<int>(CommandResult.NOTFOUND);
            }
            try
            {
                if (worksImportRequest.Status != WorksImportRequestStatus.Rollback)
                {
                    Logger.LogInformation((int)WorksImportRequestFeatureEvent.UpdateNotFound,
                        $"Attempted to roll back request {request.Id} which has a status of {worksImportRequest.Status}");
                    return Result.Fail<int>(
                        $"Unable to roll back request {request.Id} which has a status of {worksImportRequest.Status}");
                }

                worksImportRequest.Status = WorksImportRequestStatus.ProcessingRollBack;
                await OscarContext.SaveChangesAsync(cancellationToken);

                var worksList = OscarContext.Works.Where(w => w.WorksImportRequest != null && w.WorksImportRequest.Id == request.Id).ToList();
                var deleteCount = worksList.Count();

                DeleteImportedWorks(request.Id);

                worksImportRequest.Status = WorksImportRequestStatus.RolledBack;
                await OscarContext.SaveChangesAsync(cancellationToken);

                return Result.Ok(deleteCount);
            }
            catch (SqlException ex)
            {
                worksImportRequest.Status = WorksImportRequestStatus.Error;
                await OscarContext.SaveChangesAsync(cancellationToken);
                throw;
            }
        }

        public int DeleteImportedWorks(int worksImportRequestId)
        {
            var worksImportRequestIdParameter = new SqlParameter("@WorksImportRequestId", worksImportRequestId);

            return OscarContext.Database.ExecuteSqlRaw("EXEC [dbo].[sp_DeleteImportedWorks] @WorksImportRequestId", worksImportRequestIdParameter);
        }
    }
}