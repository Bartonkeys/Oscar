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
    public class AddCatalogueCommand : IRequest<Result<CatalogueDto>>
    {
        public CatalogueAddDto catalogueAddDto { get; set; }
    }

    public class AddCatalogueCommandHandler : AbstractBaseHandler<AddCatalogueCommand, CatalogueDto>
    {
        public AddCatalogueCommandHandler(OscarContext oscarContext, IMapper mapper, IValidator<AddCatalogueCommand> validator, ILogger<AddCatalogueCommand> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result<CatalogueDto>> HandleRequest(AddCatalogueCommand request, CancellationToken cancellationToken)
        {
            var catalogue = Mapper.Map<Oscar.Core.Entities.Catalogue>(request.catalogueAddDto);
            catalogue.Client = OscarContext.Clients.Single(c => c.Id == request.catalogueAddDto.ClientId);

            OscarContext.Add(catalogue);
            await OscarContext.SaveChangesAsync(cancellationToken);

            Logger.LogInformation((int)CatalogueFeatureEvent.Add, CommandResult.SUCCESS);
            catalogue = await OscarContext.Catalogues.Include(c => c.Client).FirstOrDefaultAsync(c => c.Id == catalogue.Id);

            return Result.Ok(Mapper.Map<CatalogueDto>(catalogue));
        }

    }
}
