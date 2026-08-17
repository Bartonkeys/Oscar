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
    public class AmendBulkRightsCommand : IRequest<Result>
    {
        public int? ClientId { get; set; }
        public int? CatalogueId { get; set; }

        public DateTime? StartOfRight { get; set; }
        public DateTime? EndOfRight { get; set; }
        public DateTime? StartOfValidity { get; set; }
        public DateTime? EndOfValidity { get; set; }
    }

    public class AmendBulkRightsCommandHandler : SimpleAbstractBaseHandler<AmendBulkRightsCommand>
    {
        public AmendBulkRightsCommandHandler(OscarContext oscarContext, IMapper mapper, IValidator<AmendBulkRightsCommand> validator, ILogger<AmendBulkRightsCommand> logger) : base(oscarContext, mapper, validator, logger)
        {
        }

        protected override async Task<Result> HandleRequest(AmendBulkRightsCommand request, CancellationToken cancellationToken)
        {
            var catalogue = await OscarContext
                .Catalogues
                .Include(r => r.Rights)
                .Include(r => r.Works).ThenInclude(w => w.Rights)
                .AsSplitQuery()
                .SingleAsync(c => c.Id == request.CatalogueId, cancellationToken);

            var rightsToUpdate = catalogue.Works.SelectMany(r => r.Rights).ToList();

            foreach (var rightToUpdate in rightsToUpdate)
                CopyRightsFromTo(request, rightToUpdate);

            foreach (var catalogueRight in catalogue.Rights)
                CopyRightsFromTo(request, catalogueRight);

            await OscarContext.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }


        public void CopyRightsFromTo(AmendBulkRightsCommand request, Right destination)
        {
            if(request.StartOfRight != null && request.StartOfRight < destination.StartOfRight) 
                destination.StartOfRight = request.StartOfRight.Value;

            if (request.EndOfRight != null && request.EndOfRight > destination.EndOfRight)
                destination.EndOfRight = request.EndOfRight.Value;

            if (request.StartOfValidity != null && request.StartOfValidity < destination.StartOfValidity)
                destination.StartOfValidity = request.StartOfValidity.Value;

            if (request.EndOfValidity != null && request.EndOfValidity > destination.EndOfValidity )
                destination.EndOfValidity = request.EndOfValidity.Value;

            destination.BulkAmendRights = DateTime.Now;
        }
    }
}
