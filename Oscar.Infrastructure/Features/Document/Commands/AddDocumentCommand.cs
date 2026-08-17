using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;
using Oscar.Infrastructure.Features.Common.Contracts;

namespace Oscar.Infrastructure.Features.Document.Commands
{
    public class AddDocumentCommand : IRequest<Result<DocumentDto>>
    {
        public DocumentDto DocumentDto { get; set; }
    }

    public class AddDocumentCommandHandler : AbstractBaseHandler<AddDocumentCommand, DocumentDto>
    {
        private IContainerService _containerService;
        private readonly IConfiguration config;


        public AddDocumentCommandHandler(
            OscarContext oscarContext,
            IMapper mapper,
            IValidator<AddDocumentCommand> validator,
            ILogger<AddDocumentCommand> logger,
            IContainerService containerService,
            IConfiguration configuration
            ) : base(oscarContext, mapper, validator, logger)
        {
            _containerService = containerService;
            config = configuration;
        }

        protected override async Task<Result<DocumentDto>> HandleRequest(AddDocumentCommand request, CancellationToken cancellationToken)
        {
            var fileExtension = System.IO.Path.GetExtension(request.DocumentDto.FormFile.FileName);
            var document = Mapper.Map<Oscar.Core.Entities.Document>(request.DocumentDto);

            switch (request.DocumentDto.DocumentType)
            {
                case Core.Enums.DocumentType.Works:
                    document.Works = OscarContext.Works.First(x => x.Id == request.DocumentDto.OwnerId);
                    break;
                case Core.Enums.DocumentType.Client:
                    document.Client = OscarContext.Clients.First(x => x.Id == request.DocumentDto.OwnerId);
                    break;
            }

            OscarContext.Add(document);

            await OscarContext.SaveChangesAsync(cancellationToken);

            var uploadResult = await _containerService.UploadAsync(request.DocumentDto.FormFile, ContainerName.DOCUMENTS, document.Id, fileExtension, request.DocumentDto.DocumentType.ToString(), cancellationToken);
            if (uploadResult.IsSuccess)
            {

                document.FileName = uploadResult.Value;
                await OscarContext.SaveChangesAsync(cancellationToken);
                Logger.LogInformation((int)DocumentFeatureEvent.Add, CommandResult.SUCCESS);

                var documentDto = Mapper.Map<DocumentDto>(document);
                documentDto.PublicUrl = config["oscarstorage:blob"] + ContainerName.DOCUMENTS + Path.DirectorySeparatorChar + documentDto.DocumentType + Path.DirectorySeparatorChar + documentDto.FileName;

                return Result.Ok(documentDto);
                
            }
            await OscarContext.SaveChangesAsync(cancellationToken);
            return Result.Fail<DocumentDto>(CommandResult.ERROR);
        }

        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

    }
}
