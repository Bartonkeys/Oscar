using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;
using Oscar.Infrastructure.Features.Common.Contracts;
using Oscar.Infrastructure.Features.Common.Services;

namespace Oscar.Infrastructure.Features.Document.Commands
{
    public class DeleteDocumentCommand : IRequest<Result<bool>>
    {
        public int Id { get; set; }
    }

    public class DeleteDocumentCommandHandler : AbstractBaseHandler<DeleteDocumentCommand, bool>
    {
        private IContainerService _containerService;

        public DeleteDocumentCommandHandler(
            OscarContext oscarContext,
            IMapper mapper, 
            IValidator<DeleteDocumentCommand> validator,
            ILogger<DeleteDocumentCommand> logger,
            IContainerService containerService
            ) : base(oscarContext, mapper, validator, logger)
        {
            _containerService = containerService;
        }

        protected override async Task<Result<bool>> HandleRequest(DeleteDocumentCommand request, CancellationToken cancellationToken)
        {
            var document = OscarContext.Documents.FirstOrDefault(x => x.Id == request.Id);

            if (document == null)
            {
                return Result.Fail<bool>("Document not found");
            }
             
            var blobNameToDelete = $"{document.DocumentType.ToString()}{Path.DirectorySeparatorChar}{document.FileName}";
            var deleteResult = await _containerService.DeleteBlob(ContainerName.DOCUMENTS, blobNameToDelete, cancellationToken);
            if (deleteResult.IsSuccess)
            {
                Logger.LogInformation((int)DocumentFeatureEvent.DeleteAzureBlob, CommandResult.SUCCESS + " - Successfully deleted blob: " + blobNameToDelete);
            }
            else
            {
                // NB: Do not fail the DeleteDocumentCommand if Azure blob delete errors out
                Logger.LogInformation((int)DocumentFeatureEvent.DeleteAzureBlob, CommandResult.ERROR + " - Could not delete blob: " + blobNameToDelete);
            }


            OscarContext.Documents.Remove(document);
            await OscarContext.SaveChangesAsync();

            Logger.LogInformation((int)DocumentFeatureEvent.DeleteDocument, CommandResult.SUCCESS);
            return Result.Ok(Mapper.Map<bool>(true));
        }
    }
}

