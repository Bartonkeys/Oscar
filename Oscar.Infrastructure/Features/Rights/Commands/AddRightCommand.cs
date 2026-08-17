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
using Oscar.Infrastructure.Features.Rights.Queries;
using static Oscar.Core.Common.Constants;

namespace Oscar.Infrastructure.Features.Rights.Commands
{
    public class AddRightCommand : IRequest<Result<RightDto>>
    {
        public RightAddDto RightAddDto { get; set; }
        public RightsSource RightsSource { get; set; }
    }

    public class AddRightCommandHandler : AbstractBaseHandler<AddRightCommand, RightDto>
    {
        private readonly IMediator _mediator;

        public AddRightCommandHandler(OscarContext oscarContext, IMapper mapper, IValidator<AddRightCommand> validator, ILogger<AddRightCommand> logger, IMediator mediator) : base(oscarContext, mapper, validator, logger)
        {
            _mediator = mediator;
        }

        protected override async Task<Result<RightDto>> HandleRequest(AddRightCommand request, CancellationToken cancellationToken)
        {
            var rightsType = await OscarContext.RightsTypes.FirstAsync(rt => rt.Id == request.RightAddDto.TypeID);
            var work = await OscarContext.Works
                .Include(w => w.Catalogues)
                .Include(w => w.Clients)
                .AsSplitQuery()
                .FirstOrDefaultAsync(w => w.Id == request.RightAddDto.WorksID);

            //Apply validations using client rights for the rights getting created from works or from catalogues
            if (request.RightsSource == RightsSource.Works || 
                request.RightsSource == RightsSource.Catalogue)
            {
                var source = request.RightsSource == RightsSource.Catalogue ? "Client" : "Catalogue";

                int clientId;
                if (request.RightAddDto.ClientID == 0)
                {
                    //client may not have been associated to work yet hence it could be null
                    clientId = work?.Clients?.FirstOrDefault()?.Id ??  0;
                }
                else
                {
                    clientId = request.RightAddDto.ClientID;
                }

                if (clientId > 0)
                {
                    var clientCataloguesCount = OscarContext.Catalogues
                        .Where(c => c.Client.Id == clientId)
                        .Count();

                    //Don't validate for client-catalogue rights but validate for rights added/updated from other catalogues and from works
                    //If there's only 1 catalogue available then it will be treated as client catalogue
                    if (clientCataloguesCount > 1)
                    {
                        var clientRights = await GetClientRights(clientId);

                        var clientOnlyRights = clientRights.GetClientOnlyRights();

                        //validate rights for catalogues other than client catalogue
                        //other catalogue rights should be same or less right when compare to client catalogue rights
                        if (request.RightAddDto.CatalogueID != clientOnlyRights?.First()?.Catalogue?.Id)
                        {
                            var canAddRightType = await IsRightTypeAllowedToBeAdded(request, clientOnlyRights);
                            if (!canAddRightType)
                            {
                                return Result.Fail<RightDto>($"Right {rightsType.Name} is not present in {source} Rights");
                            }
                            var excludedCountryName = await IsRightCountryExcluded(request, clientOnlyRights);
                            if (excludedCountryName != string.Empty)
                            {
                                return Result.Fail<RightDto>($"Right for {excludedCountryName} is not present in {source} Rights");
                            }
                        }
                    }
                }
            }

            var right = new Right
            {
                Type = rightsType,
                Client = OscarContext.Clients.FirstOrDefault(c => c.Id == request.RightAddDto.ClientID),
                StartOfRight = request.RightAddDto.Start,
                EndOfRight = request.RightAddDto.End,
                StartOfValidity = request.RightAddDto.StartValidity,
                EndOfValidity = request.RightAddDto.EndValidity,
                Notations = request.RightAddDto.Notations,
                Percentage = request.RightAddDto.Percentage,
                CreationDate = request.RightAddDto.Creation,
                Work = work,
                Countries = new List<Core.Entities.Country>()
            };


            if (request.RightAddDto.CatalogueID != null && request.RightAddDto.CatalogueID.Value > 0)
                right.Catalogue = OscarContext.Catalogues.FirstOrDefault(c => c.Id == request.RightAddDto.CatalogueID);

            RightsHelper.SetChannelRights(right, request.RightAddDto.ChannelIds, OscarContext);
            RightsHelper.SetLanguageRights(right, request.RightAddDto.LanguageIds, OscarContext);
            RightsHelper.SetCollection<Core.Entities.Country>(right.Countries, request.RightAddDto.CountryIds, OscarContext);

            OscarContext.Add(right);
            await OscarContext.SaveChangesAsync(cancellationToken);

            Logger.LogInformation((int)RightFeatureEvent.Add, CommandResult.SUCCESS);
            return Result.Ok(Mapper.Map<RightDto>(right));
        }

        private async Task<bool> IsRightTypeAllowedToBeAdded(AddRightCommand request, IEnumerable<RightDto> clientRights)
        {
            return clientRights.Any(r => r.Type.Id == request.RightAddDto.TypeID);
        }

        //Countries that are excluded from rights would have been saved with 0% in client rights
        private async Task<string> IsRightCountryExcluded(AddRightCommand request, IEnumerable<RightDto> clientRights)
        {
            string excludedCountries = string.Empty;
            var excludedRightsCountries = clientRights.Where(r => (r.Type.Id == request.RightAddDto.TypeID && 
                                                                   r.Catalogue.Id == request.RightAddDto.CatalogueID) && 
                                                                   r.Percentage == 0)
                                          .SelectMany(x => x.Countries);

            foreach (var excludedCountry in excludedRightsCountries)
            {
                if (request.RightAddDto.CountryIds.Any(countryId => countryId == excludedCountry?.Id))
                {
                    if (excludedCountries == string.Empty)
                        excludedCountries = excludedCountry.Name;
                    else
                        excludedCountries += ", " + excludedCountry.Name;
                }
            }

            return excludedCountries;
        }

        private async Task<IEnumerable<RightDto>> GetClientRights(int clientId)
        {
            var rights = (await _mediator.Send(new GetRightsByClientIdQuery
            {
                ClientId = clientId,
            })).Value;

            return rights;
        }
    }
}
