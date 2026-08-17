using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;

namespace Oscar.Infrastructure.Features.Document.Queries
{
    public class GetDocumentQuery: BaseTableQuery, IRequest<Result<IEntityTable<DocumentDto>>>
    {
        public int Id { get; set; }
    }

    public class GetDocumentQueryHandler : AbstractBaseHandler<GetDocumentQuery, IEntityTable<DocumentDto>>
    {
        private readonly IConfiguration config;

        public GetDocumentQueryHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetDocumentQuery> validator, ILogger<GetDocumentQuery> logger, IConfiguration configuration) : base(oscarContext, mapper, validator, logger)
        {
            config = configuration;
        }

        protected override async Task<Result<IEntityTable<DocumentDto>>> HandleRequest(GetDocumentQuery request, CancellationToken cancellationToken)
        {
            Logger.LogInformation((int)DocumentFeatureEvent.Get, CommandResult.SUCCESS);

            var documents = OscarContext.Documents;
            var total = documents.Count();

            var documentDtos = documents.Select(c => Mapper.Map<DocumentDto>(c));

            foreach (var doc in documentDtos)
            {
                doc.PublicUrl = config["oscarstorage:blob"] + ContainerName.DOCUMENTS + Path.DirectorySeparatorChar + doc.DocumentType.ToString() + Path.DirectorySeparatorChar + doc.FileName;

            }

            return Result.Ok(EntityTable<DocumentDto>.Create(documentDtos).WithTotal(total));
        }
        
    }
}
