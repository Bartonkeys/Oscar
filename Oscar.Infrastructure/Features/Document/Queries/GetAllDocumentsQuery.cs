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
using System.Linq.Expressions;
using Oscar.Infrastructure.Features.Common.Contracts;
using Microsoft.Extensions.Configuration;

namespace Oscar.Infrastructure.Features.Document.Queries
{
    public class GetAllDocumentsQuery: IRequest<Result<IEnumerable<DocumentDto>>>
    {
    }
    
    public class GetAllDocumentsHandler : AbstractBaseHandler<GetAllDocumentsQuery, IEnumerable<DocumentDto>>
    {
        private readonly IConfiguration config;

        public GetAllDocumentsHandler(OscarContext oscarContext, IMapper mapper, IValidator<GetAllDocumentsQuery> validator, 
            ILogger<GetAllDocumentsQuery> logger, IConfiguration configuration) 
            : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<IEnumerable<DocumentDto>>> HandleRequest(GetAllDocumentsQuery request, CancellationToken cancellationToken)
        {
            var documents = OscarContext.Documents.ToList();

            var documentDtos = documents.Select(c => Mapper.Map<DocumentDto>(c));

            foreach (var doc in documentDtos)
            {
                doc.PublicUrl = config["oscarstorage:blob"] + ContainerName.DOCUMENTS + Path.DirectorySeparatorChar + doc.DocumentType.ToString() + Path.DirectorySeparatorChar + doc.FileName;

            }

            Logger.LogInformation((int)DocumentFeatureEvent.Get, CommandResult.SUCCESS);

            return Result.Ok(documentDtos);
        }

    }
}
