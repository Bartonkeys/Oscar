using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Core.Entities;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Common;

namespace Oscar.Infrastructure.Features.Rights.Commands
{
    public class UpdateBulkRightsCommand : IRequest<Result>
    {
        public int? ClientId { get; set; }
        public int? CatalogueId { get; set; }
        public List<RightDto> Rights { get; set; }
    }

    public class UpdateBulkRightsCommandHandler : SimpleAbstractBaseHandler<UpdateBulkRightsCommand>
    {
        public UpdateBulkRightsCommandHandler(OscarContext oscarContext, IMapper mapper, IValidator<UpdateBulkRightsCommand> validator, ILogger<UpdateBulkRightsCommand> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result> HandleRequest(UpdateBulkRightsCommand request, CancellationToken cancellationToken)
        {
            var amendedRights = Mapper.Map<List<Right>>(request.Rights);

            var catalogue = await OscarContext
                .Catalogues
                .Include(r => r.Rights)!.ThenInclude(c => c.LanguageRights)
                .Include(r => r.Rights)!.ThenInclude(c => c.ChannelRights)
                .Include(r => r.Works).ThenInclude(w => w.Rights)!.ThenInclude(c => c.Type)
                .Include(r => r.Works).ThenInclude(w => w.Rights)!.ThenInclude(c => c.Countries)
                .Include(r => r.Works).ThenInclude(w => w.Rights)!.ThenInclude(c => c.LanguageRights)
                .Include(r => r.Works).ThenInclude(w => w.Rights)!.ThenInclude(c => c.ChannelRights)
                .AsSplitQuery()
                .SingleAsync(c => c.Id == request.CatalogueId, cancellationToken: cancellationToken);

            foreach (var amendedRight in amendedRights)
            {
                var originalRight = await OscarContext
                    .Rights
                    .Include(c => c.Countries)
                    .Include(t => t.Type)
                    .SingleAsync(r => r.Id == amendedRight.Id, cancellationToken: cancellationToken);

                if (originalRight.Equals(amendedRight)) continue;

                var rightsToUpdate = catalogue.Works.SelectMany(r => r.Rights).Where(r => r.Equals(originalRight)).ToList();

                foreach (var rightToUpdate in rightsToUpdate)
                {
                    CopyRightsFromTo(amendedRight, rightToUpdate);
                    var catalogueRight = catalogue.Rights.First(r => r.Id == originalRight.Id);
                    CopyRightsFromTo(amendedRight, catalogueRight);
                }

            }

            await OscarContext.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }


        public void CopyRightsFromTo(Right source, Right destination)
        {
            destination.Percentage = source.Percentage;

            if(destination.StartOfRight < source.StartOfRight) 
                destination.StartOfRight = source.StartOfRight;

            if (destination.EndOfRight > source.EndOfRight)
                destination.EndOfRight = source.EndOfRight;

            if (destination.StartOfValidity < source.StartOfValidity)
                destination.StartOfValidity = source.StartOfValidity;

            if (destination.EndOfValidity > source.EndOfValidity)
                destination.EndOfValidity = source.EndOfValidity;

            destination.BulkAmendRights = DateTime.Now;

            var sourceCountries = OscarContext.Countries.Where(c => source.Countries.Contains(c)).ToList();
            destination.Countries = sourceCountries;

            destination.LanguageRights.Clear();
            foreach (var sourceLanguageRight in source.LanguageRights)
            {
                var languageRight = new LanguageRights
                {
                    Language = sourceLanguageRight.Language,
                    Right = destination
                };
                OscarContext.Entry(languageRight).State = EntityState.Added;
            }

            destination.ChannelRights.Clear();
            foreach (var sourceChannelRight in source.ChannelRights)
            {
                var channelRight = new ChannelRights
                {
                    Channel = sourceChannelRight.Channel,
                    Right = destination
                };
                OscarContext.Entry(channelRight).State = EntityState.Added;
            }
        }
    }
}
