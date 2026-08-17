using AutoMapper;
using BartonKeys.Functional;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Oscar.Core.DTOs;
using Oscar.Data.Context;
using Oscar.Infrastructure.Features.Rights.Commands;
using Oscar.Infrastructure.Features.Rights.Queries;

namespace Oscar.Infrastructure.Features.Common;

public abstract class AbstractWithMediatorHandler<T, TR, TU> : AbstractBaseHandler<T, TR>
    where T : IRequest<Result<TR>>
    where TU : WorksUpdateDto
{
    protected readonly IMediator Mediator;

    public AbstractWithMediatorHandler(OscarContext oscarContext, IMapper mapper,
        IValidator<T> validator, ILogger<T> logger, IMediator mediator) : base(oscarContext, mapper, validator, logger)
    {
        Mediator = mediator;
    }

    protected async Task InheritRights(TU request, int worksId, int? WorksProductionYear)
    {
        if (OscarContext.Rights.Any(r => r.Work.Id == worksId))
            return;

        if (request.RightIds == null || request.RightIds.Count == 0)
        {
            var catalogueIds = request.CatalogueIds;
            var clientIds = request.ClientIds;
            if (clientIds != null && clientIds.Any())
            {
                var inheritedWorksRights = (await Mediator.Send(new GetRightsByClientIdQuery
                    {
                        ClientId = clientIds.First(),
                        CatalogueId = catalogueIds != null && catalogueIds.Any() ? catalogueIds.First() : null
                    }
                )).Value;

                if (catalogueIds == null || !catalogueIds.Any())
                    inheritedWorksRights = inheritedWorksRights.GetClientOnlyRights();

                foreach (var inheritedWorksRight in inheritedWorksRights)
                {
                    await Mediator.Send(new AddRightCommand
                    {
                        RightAddDto = new RightAddDto
                        {
                            CatalogueID = inheritedWorksRight.Catalogue?.Id,
                            ChannelIds = inheritedWorksRight.ChannelRights.Select(c => c.Channel.Id).ToList(),
                            ClientID = clientIds.First(),
                            CountryIds = inheritedWorksRight.Countries.Select(c => c.Id).ToList(),
                            Start = new DateTime(Convert.ToInt32(WorksProductionYear), 1, 1),
                            End = inheritedWorksRight.EndOfRight,
                            StartValidity = new DateTime(Convert.ToInt32(WorksProductionYear), 1, 1),
                            EndValidity = inheritedWorksRight.EndOfValidity,
                            TypeID = inheritedWorksRight.TypeId,
                            Percentage = inheritedWorksRight.Percentage,
                            Notations = inheritedWorksRight.Notations,
                            WorksID = worksId,
                            LanguageIds = inheritedWorksRight.LanguageRights.Select(l => l.Language.Id).ToList()
                        }
                    });
                }
            }
        }
    }
}