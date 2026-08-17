using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;

namespace Oscar.Infrastructure.Features.WorksImport.Queries
{
    public class GetWorksImportRequestByIdQuery: BaseTableQuery, IRequest<Result<WorksImportRequestDto>>
    {
        public int Id { get; set; }
    }

    public class WorksImportRequestByIdHandler : AbstractBaseHandler<GetWorksImportRequestByIdQuery, WorksImportRequestDto>
    {
        public WorksImportRequestByIdHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetWorksImportRequestByIdQuery> validator, ILogger<GetWorksImportRequestByIdQuery> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<WorksImportRequestDto>> HandleRequest(GetWorksImportRequestByIdQuery request, CancellationToken cancellationToken)
        {
            OscarContext.ChangeTracker.LazyLoadingEnabled = false;
            var worksImportRequest = await OscarContext.WorksImportRequests.AsNoTracking().FirstOrDefaultAsync(w => w.Id == request.Id);

            Logger.LogInformation((int)WorksImportRequestFeatureEvent.Get, CommandResult.SUCCESS);
            return Result.Ok(Mapper.Map<WorksImportRequestDto>(worksImportRequest));
        }

    }
}
