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
    public class GetDocumentsByTypeQuery : BaseTableQuery, IRequest<Result<IEntityTable<DocumentDto>>>
    {
        public Core.Enums.DocumentType DocumentType { get; set; }
    }

    public class GetDocumentsByTypeQueryHandler : AbstractBaseHandler<GetDocumentsByTypeQuery, IEntityTable<DocumentDto>>
    {
        private readonly IConfiguration config;

        public GetDocumentsByTypeQueryHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetDocumentsByTypeQuery> validator, ILogger<GetDocumentsByTypeQuery> logger, IConfiguration configuration) : base(oscarContext, mapper, validator, logger)
        {
            config = configuration;
        }

        protected override async Task<Result<IEntityTable<DocumentDto>>> HandleRequest(GetDocumentsByTypeQuery request, CancellationToken cancellationToken)
        {
            Logger.LogInformation((int)DocumentFeatureEvent.Get, CommandResult.SUCCESS);

            var documents = OscarContext.Documents
                .Where(x => x.DocumentType.Equals(request.DocumentType));
            var total = documents.Count();

            
            var documentDtos = documents.Select(c => Mapper.Map<DocumentDto>(c));

            foreach (var doc in documentDtos)
            {
                doc.PublicUrl  = config["oscarstorage:blob"] + ContainerName.DOCUMENTS + Path.DirectorySeparatorChar + doc.DocumentType.ToString() + Path.DirectorySeparatorChar + doc.FileName;

            }

            return Result.Ok(EntityTable<DocumentDto>.Create(documentDtos).WithTotal(total));
        }

    }
}
