using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BartonKeys.Extensions;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;
using Oscar.Infrastructure.Features.WorksImport.Services;
using static BartonKeys.Functional.Result;

namespace Oscar.Infrastructure.Features.WorksImport.Commands
{
    public class WorksImportCommand : IRequest<Result>
    {
        public int WorksImportRequestId { get; set; }
        public WorksImportRequestStatus Status { get; set; }
    }

    public class WorksImportCommandHandler : SimpleAbstractBaseHandler<WorksImportCommand>
    {
        private readonly IWorksImportService _worksImportService;

        public WorksImportCommandHandler(OscarContext oscarContext, 
            IMapper mapper, IValidator<WorksImportCommand> validator, 
            ILogger<WorksImportCommand> logger,
            IWorksImportService worksImportService) : base(oscarContext, mapper, validator, logger)
        {
            _worksImportService = worksImportService;
        }

        protected override async Task<Result> HandleRequest(WorksImportCommand request, CancellationToken cancellationToken)
        {
            var worksImportRequest = (await OscarContext.WorksImportRequests
                .Include(c => c.Catalogue)
                .Include(c => c.Client).ThenInclude(c => c.Catalogues)
                .Include(w => w.WorksImports)
                .SingleOrDefaultAsync(r => r.Id == request.WorksImportRequestId, cancellationToken: cancellationToken))
                .ToMaybe();

            if (!worksImportRequest.HasValue)
                return Fail("Request Not Found");

            worksImportRequest.Value.Status = WorksImportRequestStatus.Processing;
            await OscarContext.SaveChangesAsync(cancellationToken);

            var records = worksImportRequest.Value!.WorksImports;

            if(request.Status != WorksImportRequestStatus.ReSubmit)
                await _worksImportService.CheckForDuplicates(records, cancellationToken);

            if (records!.Any(r => r.PossibleDuplicate) && request.Status != WorksImportRequestStatus.ReSubmit)
            {
                worksImportRequest.Value.Status = WorksImportRequestStatus.PossibleDuplicates;
                await OscarContext.SaveChangesAsync(cancellationToken);
                return Ok();
            }
            else
            {
                var writeRecordsResult = await _worksImportService.WriteWorksRecords(worksImportRequest.Value, worksImportRequest.Value.Client, worksImportRequest.Value.Catalogue);

                worksImportRequest.Value.Status = writeRecordsResult.IsSuccess ? WorksImportRequestStatus.Success : WorksImportRequestStatus.Error;
                await OscarContext.SaveChangesAsync(cancellationToken);
                return writeRecordsResult.IsSuccess ? Ok() : Fail(writeRecordsResult.Error);
            }
        }
    }
}
