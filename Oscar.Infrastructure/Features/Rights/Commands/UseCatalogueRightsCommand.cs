using AutoMapper;
using Azure.Core;
using BartonKeys.Extensions;
using BartonKeys.Functional;
using DocumentFormat.OpenXml.ExtendedProperties;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;
using Oscar.Core.Enums;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;
using Oscar.Infrastructure.Features.Rights.Queries;

namespace Oscar.Infrastructure.Features.Rights.Commands
{
    public class UseCatalogueRightsCommand : IRequest<Result<IEnumerable<RightDto>>>
    {
        public int Id { get; set; }
    }

    public class UseCatalogueRightsCommandHandler : AbstractBaseHandler<UseCatalogueRightsCommand, IEnumerable<RightDto>>
    {
        private readonly IMediator _mediator;

        public UseCatalogueRightsCommandHandler(OscarContext oscarContext, IMapper mapper, IValidator<UseCatalogueRightsCommand> validator, ILogger<UseCatalogueRightsCommand> logger, IMediator mediator) 
            : base(oscarContext, mapper, validator, logger)
        {
            _mediator = mediator;
        }

        protected override async Task<Result<IEnumerable<RightDto>>> HandleRequest(UseCatalogueRightsCommand request, CancellationToken cancellationToken)
        {
            var works = await OscarContext
                .Works
                .Include(r => r.Rights)
                .Include(c => c.Catalogues).ThenInclude(c => c.Client)
                .AsSplitQuery()
                .SingleAsync(w => w.Id == request.Id);

            var maybeCatalogue = works.Catalogues.FirstOrDefault().ToMaybe();

            if (!maybeCatalogue.HasValue) return Result.Fail< IEnumerable<RightDto>>("No Catalogue found");

            var catalogueRights = await _mediator.Send(new GetRightsByClientIdQuery
                { ClientId = maybeCatalogue.Value.Client.Id, CatalogueId = maybeCatalogue.Value.Id }, cancellationToken);

            if (!catalogueRights.Value.Any()) return Result.Fail< IEnumerable<RightDto>>("No Catalogue Rights exist");

            await DeleteWorksRights(works.Rights, cancellationToken);
                
            var rights = await AddCatalogueRights(works, maybeCatalogue.Value, catalogueRights.Value, cancellationToken);

            return Result.Ok(rights);
        }

        private async Task DeleteWorksRights(IEnumerable<Right> worksRights, CancellationToken cancellationToken)
        {
            foreach (var worksRight in worksRights)
                await _mediator.Send(new DeleteRightCommand
                {
                    RightDeleteDto = new RightDeleteDto { ID = worksRight.Id }
                }, cancellationToken);
        }

        private async Task<IEnumerable<RightDto>> AddCatalogueRights(Core.Entities.Works works, Core.Entities.Catalogue catalogue, IEnumerable<RightDto> catalogueRights, CancellationToken cancellationToken)
        {
            var rights = new List<RightDto>();

            foreach (var catalogueRight in catalogueRights)
            {
                var right = new Right
                {
                    Type = OscarContext.RightsTypes.Single(r => r.Id == catalogueRight.TypeId),
                    Client = catalogue.Client,
                    StartOfRight = works.ProductionYear != null ? new DateTime(works.ProductionYear.Value, 1, 1) : catalogueRight.StartOfRight,
                    EndOfRight = catalogueRight.EndOfRight,
                    StartOfValidity = works.ProductionYear != null ? new DateTime(works.ProductionYear.Value, 1, 1): catalogueRight.StartOfValidity,
                    EndOfValidity = catalogueRight.EndOfValidity,
                    Notations = catalogueRight.Notations,
                    Percentage = catalogueRight.Percentage,
                    CreationDate = DateTime.Now,
                    Work = works,
                    Countries = new List<Core.Entities.Country>(),
                    Catalogue = catalogue
                };

                RightsHelper.SetChannelRights(right, catalogueRight.ChannelRights.Select(cr => cr.Channel.Id).ToList(), OscarContext);
                RightsHelper.SetLanguageRights(right, catalogueRight.LanguageRights.Select(l => l.Language.Id).ToList(), OscarContext);
                RightsHelper.SetCollection(right.Countries, catalogueRight.Countries.Select(c => c.Id).ToList(), OscarContext);

                OscarContext.Add(right);
                rights.Add(Mapper.Map<RightDto>(right));
            }
            await OscarContext.SaveChangesAsync(cancellationToken);
            return rights;
        }
    }
}
